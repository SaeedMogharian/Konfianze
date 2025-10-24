using RTLTMPro;
using UnityEngine;

namespace __Lamboo_packages.Connection.Scripts
{
    public enum PlayerRole
    {
        A,
        B,
        C,
        Unknown
    }

    public sealed class MatchFoundInfo
    {
        public string MatchId;
        public PlayerRole AssignedRole = PlayerRole.Unknown;
    }

    public sealed class ChatEnvelope
    {
        public string MatchId;
        public string SenderUserId;
        public string Text;
        public string Tag;
        public bool IsSystem;
        public long UnixTimeMs;
    }

    public sealed class MatchStateEnvelope
    {
        public long OpCode;
        public string Json;
        public byte[] Binary;
    }

    public class ConnectionController : MonoBehaviour
    {
        [SerializeField] private Connection connection;
        [SerializeField] private RTLTextMeshPro temptext;

        private void OnEnable()
        {
            _ = connection.InitializeAsync();
            connection.Connected += HandleConnecting;
            connection.MatchFound += HandleMatchFound;
            // connection.
            Debug.Log("-------------- Connection Initialized");
            temptext.text = "در حال اتصال";
        }

        private void OnDisable()
        {
            connection.Connected -= HandleConnecting;
            connection.MatchFound -= HandleMatchFound;
        }

        private void HandleConnecting()
        {
            connection.StartMatchmakingAsync();
            // TODO: ANNOUNCE PLAYER
            temptext.text = "در جست و جوی مچ!";
        }
        
        private void HandleMatchFound(MatchFoundInfo obj)
        {
            connection.JoinMatchChatAsync();
            // TODO: ANNOUNCE PLAYER
            temptext.text = "اتصال به چت";
        }
    }
}