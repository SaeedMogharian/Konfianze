using System;
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

        private void OnEnable()
        {
            _ = connection.InitializeAsync();
        }
    }
}