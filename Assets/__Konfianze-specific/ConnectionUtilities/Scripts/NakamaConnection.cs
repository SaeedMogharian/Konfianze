using System;
using System.Text;
using Nakama;
using Nakama.TinyJson;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace __Konfianze_specific.ConnectionUtilities.Scripts
{
    public sealed class NakamaConnection : Connection
    {
        [Header("Nakama")] [SerializeField] private string host = "127.0.0.1";
        [SerializeField] private int port = 7350;
        [SerializeField] private bool useSSL;
        [SerializeField] private string serverKey = "defaultkey";

        [Tooltip("پیشوند اتاق چت مربوط به هر مچ")]
        public string matchChatRoomPrefix = "match-";

        private IClient _client;
        private ISession _session;
        private ISocket _socket;
        private IMatch _currentMatch;
        private string _chatChannelId;
        private IChannel _chatChannel;

        public override async Task InitializeAsync(CancellationToken ct = default)
        {
            // TODO: SSL and HTTPS
            var scheme = UseSSL ? "https" : "http";
            _client = new Client(scheme, host, port, serverKey)
            {
                Logger = new UnityLogger()
            };
            await Task.CompletedTask;
        }

        public override async Task AuthenticateDeviceAsync(string deviceId, CancellationToken ct)
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

        public override async Task ConnectSocketAsync(CancellationToken ct = default)
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

            // Query ساده؛ پیشنهاد می‌کنم سمت سرور یک MatchHandler آتوریتیو داشته باشی که خودش 3 نفر رو جمع کنه
            // و role ها رو تخصیص بده. اینجا فقط تیکت می‌گیریم.
            var query = "*"; // همه
            var minCount = 3;
            var maxCount = 3;
            var stringProps = new System.Collections.Generic.Dictionary<string, string>
            {
                { "preferredRole", preferredRole.ToString() }
            };

            await _socket.AddMatchmakerAsync(query, minCount, maxCount, stringProps, null, ct);

            await Task.CompletedTask;
        }

        public override async Task CancelMatchmakingAsync(CancellationToken ct = default)
        {
            // اگر تیکت نگه‌داری می‌کنی اینجا کنسلش کن
            await Task.CompletedTask;

            await Task.CompletedTask;
        }

        public override async Task SendMatchStateAsync(long opCode, string jsonPayload, byte[] binaryPayload = null,
            CancellationToken ct = default)
        {
            EnsureSocket();
            if (_currentMatch == null) throw new InvalidOperationException("Not in a match.");
            var data = string.IsNullOrEmpty(jsonPayload) ? null : Encoding.UTF8.GetBytes(jsonPayload);
            await _socket.SendMatchStateAsync(_currentMatch.Id, opCode, data, binaryPayload);

            await Task.CompletedTask;
        }

        public override async Task JoinMatchChatAsync(CancellationToken ct = default)
        {
            EnsureSocket();
            if (_currentMatch == null) throw new InvalidOperationException("Not in a match.");

            var roomName = _currentMatch.Id;
            _chatChannel =
                await _socket.JoinChatAsync(roomName, ChannelType.Room, persistence: false, hidden: false, ct);
            _chatChannelId = _chatChannel.Id;

            await Task.CompletedTask;
        }

        public override async Task LeaveMatchChatAsync(CancellationToken ct = default)
        {
            if (!string.IsNullOrEmpty(_chatChannelId))
            {
                await _socket.LeaveChatAsync(_chatChannelId, ct);
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

            await _socket.WriteChatMessageAsync(_chatChannelId, envelope, ct);

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
            // فوروارد خام
            string json = null;
            if (st.State != null && st.State.Length > 0)
                json = Encoding.UTF8.GetString(st.State);

            var env = new MatchStateEnvelope { OpCode = st.OpCode, Json = json, Binary = null };
            OnMatchStateReceived(env);

            // مثال: اگر سرور نقش‌ها را فرستاد، ایونت MatchFound را آپدیت کن
            if (st.OpCode == OpCodes.AssignRoles && !string.IsNullOrEmpty(json))
            {
                // انتظار payload: { yourRole: "A" }
                try
                {
                    var obj = json.FromJson<RoleAssignMessage>();
                    if (Enum.TryParse<PlayerRole>(obj.yourRole, out var r))
                    {
                        OnMatchFound(new MatchFoundInfo { MatchId = _currentMatch.Id, AssignedRole = r });
                    }
                }
                catch
                {
                    /* ignore */
                }
            }
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