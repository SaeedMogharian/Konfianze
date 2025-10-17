using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace __Lamboo_packages.Connection.Scripts
{
    public abstract class Connection : ScriptableObject
    {
        [Header("Common")] [SerializeField] protected bool useSSL = false;
        public bool UseSSL => useSSL;

        public event Action Connected;
        public event Action<string> Disconnected;
        public event Action<Exception> Error;
        public event Action<MatchFoundInfo> MatchFound;
        public event Action<MatchStateEnvelope> MatchStateReceived;
        public event Action<ChatEnvelope> ChatReceived;

        // TODO: Should handled 
        public ConnectionState State { get; private set; } = ConnectionState.Disconnected;

        // TODO: Review Cancellation Token

        public async Task InitializeAsync()
        {
            await InitializeFrameworkAsync();
            // TODO: fix for web view
            var deviceID = SystemInfo.deviceUniqueIdentifier;
            await AuthenticateDeviceAsync(deviceID);
            await ConnectSocketAsync();
        }

        // Lifecycle
        protected abstract Task InitializeFrameworkAsync(CancellationToken ct = default);
        protected abstract Task AuthenticateDeviceAsync(string deviceId, CancellationToken ct = default);

        public abstract Task AuthenticatePhoneOtpAsync(string phone, string otpCode,
            CancellationToken ct = default);

        protected abstract Task ConnectSocketAsync(CancellationToken ct = default);
        public abstract Task DisconnectAsync();

        // Matchmaking
        public abstract Task StartMatchmakingAsync(PlayerRole preferredRole, CancellationToken ct = default);
        public abstract Task CancelMatchmakingAsync(CancellationToken ct = default);

        // Match State
        public abstract Task SendMatchStateAsync(long opCode, string jsonPayload, CancellationToken ct = default);

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

    public enum ConnectionState
    {
        Connected,
        Connecting,
        Disconnected,
        Error,
    }
}