using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace __Lamboo_packages.Connection.Scripts
{
    [RequireComponent(typeof(Button))]
    public class SendButton : MonoBehaviour
    {
        private Button _button;
        [SerializeField] private RTLTextMeshPro message;
        [SerializeField] private Connection connection;

        private void OnEnable()
        {
            _button ??= GetComponent<Button>();
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            connection.SendChatAsync(message.text, "USER");
        }
    }
}