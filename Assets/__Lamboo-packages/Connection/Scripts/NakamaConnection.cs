using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

namespace __Lamboo_packages.Connection.Scripts
{
    [CreateAssetMenu(fileName = "NakamaConnectionConfig", menuName = "Configs/NakamaConnectionConfig")]
    public sealed class NakamaConnection : __Lamboo_packages.Connection.Scripts.Connection
    {
        [Header("Nakama")] [SerializeField] private string host = "127.0.0.1";
        [SerializeField] private int port = 7350;
        [SerializeField] private string serverKey = "defaultkey";

        [Tooltip("پیشوند اتاق چت مربوط به هر مچ")]
        public string matchChatRoomPrefix = "match-";

        private IClient _client;
        private ISession _session;
        private ISocket _socket;
        private IMatch _currentMatch;
        private string _chatChannelId;
        private IChannel _chatChannel;

        protected override async Task InitializeFrameworkAsync(CancellationToken ct = default)
        {
            // TODO: SSL and HTTPS
            var scheme = UseSSL ? "https" : "http";
            _client = new Client(scheme, host, port, serverKey)
            {
                Logger = new UnityLogger()
            };
            await Task.CompletedTask;
        }

        protected override async Task AuthenticateDeviceAsync(string deviceId, CancellationToken ct)
        {
            _session = await _client.AuthenticateDeviceAsync(deviceId);

            await Task.CompletedTask;
        }

        public override async Task AuthenticatePhoneOtpAsync(string phone, string otpCode,
            CancellationToken ct = default)
        {
            // var payload = new { phone, otp = otpCode }.ToJson();
            // var rpc = await _client.RpcAsync("auth_phone_otp", payload, _session, ct);
            // // فرض: RPC یک session token برمی‌گردونه (یا device link). بنا به پیاده‌سازی شما:
            // // اگر RPC توکن بده:
            // if (!string.IsNullOrEmpty(rpc.Payload))
            // {
            //     var obj = rpc.Payload.FromJson<PhoneAuthResponse>();
            //     _session = Session.Restore(obj.sessionToken);
            // }
            //
            // await Task.CompletedTask;
        }

        protected override async Task ConnectSocketAsync(CancellationToken ct = default)
        {
            if (_session == null || _session.IsExpired) throw new InvalidOperationException("Authenticate first.");

            _socket = _client.NewSocket();
            _socket.Connected += OnConnected;
            _socket.Closed += () => OnDisconnected("closed");
            _socket.ReceivedError += OnError;
            _socket.ReceivedMatchState += OnReceivedMatchState;
            _socket.ReceivedMatchmakerMatched += OnMatchmakerMatched;
            _socket.ReceivedChannelMessage += OnChannelMessage;

            await _socket.ConnectAsync(_session, true);

            await Task.CompletedTask;
        }

        public override async Task DisconnectAsync()
        {
            if (_socket is { IsConnected: true }) await _socket.CloseAsync();
            _socket = null;
        }

        public override async Task StartMatchmakingAsync(PlayerRole preferredRole, CancellationToken ct = default)
        {
            EnsureSocket();

            const string query = "*";
            const int minCount = 3;
            const int maxCount = 3;
            var stringProps = new System.Collections.Generic.Dictionary<string, string>
            {
                { "preferredRole", preferredRole.ToString() }
            };

            await _socket.AddMatchmakerAsync(query, minCount, maxCount, stringProps);

            await Task.CompletedTask;
        }

        public override async Task CancelMatchmakingAsync(CancellationToken ct = default)
        {
            await Task.CompletedTask;

            await Task.CompletedTask;
        }

        public override async Task SendMatchStateAsync(long opCode, string jsonPayload, CancellationToken ct = default)
        {
            EnsureSocket();
            if (_currentMatch == null) throw new InvalidOperationException("Not in a match.");
            var data = string.IsNullOrEmpty(jsonPayload) ? null : Encoding.UTF8.GetBytes(jsonPayload);
            await _socket.SendMatchStateAsync(_currentMatch.Id, opCode, data);

            await Task.CompletedTask;
        }

        public override async Task JoinMatchChatAsync(CancellationToken ct = default)
        {
            EnsureSocket();
            if (_currentMatch == null) throw new InvalidOperationException("Not in a match.");

            var roomName = _currentMatch.Id;
            _chatChannel =
                await _socket.JoinChatAsync(roomName, ChannelType.Room, persistence: false, hidden: false);
            _chatChannelId = _chatChannel.Id;

            await Task.CompletedTask;
        }

        public override async Task LeaveMatchChatAsync(CancellationToken ct = default)
        {
            if (!string.IsNullOrEmpty(_chatChannelId))
            {
                await _socket.LeaveChatAsync(_chatChannelId);
                _chatChannelId = null;
                _chatChannel = null;
            }

            await Task.CompletedTask;
        }

        public override async Task SendChatAsync(string text, string tag = null, bool isSystem = false,
            CancellationToken ct = default)
        {
            EnsureSocket();
            if (string.IsNullOrEmpty(_chatChannelId)) throw new InvalidOperationException("Join chat first.");

            var envelope = new
            {
                kind = isSystem ? "system" : "user",
                tag = tag,
                text = text,
                ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }.ToJson();

            await _socket.WriteChatMessageAsync(_chatChannelId, envelope);

            await Task.CompletedTask;
        }

        // ---------- Internals ----------


        private void EnsureSocket()
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Socket not connected.");
        }

        private async void OnMatchmakerMatched(IMatchmakerMatched matched)
        {
            try
            {
                _currentMatch = await _socket.JoinMatchAsync(matched);
                // معمولاً نقش‌ها بلافاصله بعد جوین از سرور با MatchState (OpCodes.AssignRoles) ارسال می‌شن.
                OnMatchFound(new MatchFoundInfo
                    { MatchId = _currentMatch.Id, AssignedRole = PlayerRole.Unknown });
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        private void OnReceivedMatchState(IMatchState st)
        {
            Debug.LogError("---------- match state");
        }

        private void OnChannelMessage(IApiChannelMessage m)
        {
            try
            {
                var payload = m.Content.FromJson<ChatPayload>();
                var ce = new ChatEnvelope
                {
                    MatchId = _currentMatch?.Id,
                    SenderUserId = m.SenderId,
                    Text = payload.text,
                    Tag = payload.tag,
                    IsSystem = payload.kind == "system",
                    UnixTimeMs = payload.ts
                };
                OnChatReceived(ce);
            }
            catch
            {
                // اگر کسی پیام خام فرستاد
                var ce = new ChatEnvelope
                {
                    MatchId = _currentMatch?.Id,
                    SenderUserId = m.SenderId,
                    Text = m.Content,
                    Tag = null,
                    IsSystem = false,
                    UnixTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
                OnChatReceived(ce);
            }
        }

        private sealed class ChatPayload
        {
            public string kind;
            public string tag;
            public string text;
            public long ts;
        }

        private sealed class RoleAssignMessage
        {
            public string yourRole;
        }

        private sealed class PhoneAuthResponse
        {
            public string sessionToken;
        }
    }
}