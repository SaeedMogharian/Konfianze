using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace __Konfianze_specific.ConnectionUtilities.Scripts
{
    [CreateAssetMenu(fileName = "ConnectionConfig", menuName = "Settings/ConnectionConfig")]
    public abstract class Connection : ScriptableObject
    {
        [Header("Common")] [SerializeField] private bool uesSSL = false;
        public bool UseSSL => UseSSL;

        public event Action Connected;
        public event Action<string> Disconnected;
        public event Action<Exception> Error;
        public event Action<MatchFoundInfo> MatchFound;
        public event Action<MatchStateEnvelope> MatchStateReceived;
        public event Action<ChatEnvelope> ChatReceived;
        
        // TODO: Review Cancellation Token

        // Lifecycle
        public abstract Task InitializeAsync(CancellationToken ct = default);
        public abstract Task AuthenticateDeviceAsync(string deviceId, CancellationToken ct = default);

        public abstract Task AuthenticatePhoneOtpAsync(string phone, string otpCode,
            CancellationToken ct = default); // پیاده‌سازی از طریق RPC

        public abstract Task ConnectSocketAsync(CancellationToken ct = default);
        public abstract Task DisconnectAsync();

        // Matchmaking
        public abstract Task StartMatchmakingAsync(PlayerRole preferredRole, CancellationToken ct = default);
        public abstract Task CancelMatchmakingAsync(CancellationToken ct = default);

        // Match State
        public abstract Task SendMatchStateAsync(long opCode, string jsonPayload, byte[] binaryPayload = null,
            CancellationToken ct = default);

        // Chat
        public abstract Task JoinMatchChatAsync(CancellationToken ct = default);
        public abstract Task LeaveMatchChatAsync(CancellationToken ct = default);

        public abstract Task SendChatAsync(string text, string tag = null, bool isSystem = false,
            CancellationToken ct = default);

        protected virtual void OnConnected()
        {
            Connected?.Invoke();
        }

        protected virtual void OnDisconnected(string obj)
        {
            Disconnected?.Invoke(obj);
        }

        protected virtual void OnError(Exception obj)
        {
            Error?.Invoke(obj);
        }

        protected virtual void OnMatchFound(MatchFoundInfo obj)
        {
            MatchFound?.Invoke(obj);
        }

        protected virtual void OnMatchStateReceived(MatchStateEnvelope obj)
        {
            MatchStateReceived?.Invoke(obj);
        }

        protected virtual void OnChatReceived(ChatEnvelope obj)
        {
            ChatReceived?.Invoke(obj);
        }
    }
}