using System;
using UnityEngine;

namespace __Lamboo_packages.Connection.Scripts
{
    public class ChatController : MonoBehaviour
    {
        [SerializeField] private Connection connection;
        public event Action<string> NewMessageReceived;


        private void OnEnable()
        {
            connection.ChatReceived += HandleNewChatMessage;
        }

        private void OnDisable()
        {
            connection.ChatReceived -= HandleNewChatMessage;
        }

        private void HandleNewChatMessage(ChatEnvelope obj)
        {
            if (obj.IsSystem)
            {
                return;
            }

            NewMessageReceived?.Invoke(obj.Text);
        }
    }
}