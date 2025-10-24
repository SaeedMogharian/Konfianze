using UnityEngine;
using UnityEngine.Serialization;

namespace __Lamboo_packages.Connection.Scripts
{
    [RequireComponent(typeof(ChatController))]
    public class ChatView : MonoBehaviour
    {
        [SerializeField] private Transform contentParent;
        [SerializeField] private MessageView messagePrefabe;
        private ChatController _controller;

        private void OnEnable()
        {
            _controller ??= GetComponent<ChatController>();
            _controller.NewMessageReceived += HandleNewMessageReceived;
        }

        private void OnDisable()
        {
            _controller.NewMessageReceived -= HandleNewMessageReceived;
        }

        private void HandleNewMessageReceived(string obj)
        {
            var newInstance = Instantiate(messagePrefabe, contentParent);
            newInstance.Setup(obj);
        }
    }
}