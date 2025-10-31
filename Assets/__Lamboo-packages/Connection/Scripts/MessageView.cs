using RTLTMPro;
using UnityEngine;

namespace __Lamboo_packages.Connection.Scripts
{
    public class MessageView : MonoBehaviour
    {
        [SerializeField] private RTLTextMeshPro content;

        public void Setup(string message)
        {
            content.text = message;
        }
    }
}
