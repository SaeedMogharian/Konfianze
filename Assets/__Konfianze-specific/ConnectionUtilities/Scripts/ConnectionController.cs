using System;
using System.Collections.Generic;
using Nakama;
using Nakama.TinyJson;
using UnityEngine;

namespace __Konfianze_specific.ConnectionUtilities.Scripts
{
    public class ConnectionController : MonoBehaviour
    {
        // TODO: All except should fire releated events
        public static Action<string> OnNewMessageReceived;
        private Client _client;
        private ISocket _socket;
        private ISession _session;
        private ISocketAdapter _adapter;
        private IApiAccount _apiAccount;
        private IChannel _channel;

        private IApiGroup _apiGroup;

        #region Data Retrive

        private void Awake()
        {
            InitializeClient();
            Authenticate();
            CreateSocket();
            GetAccount();
        }

        private void InitializeClient()
        {
            _client = new Client("http", "alijomei.ir", 7350, "defaultkey")
            {
                Logger = new UnityLogger()
            };
        }

        private async void Authenticate()
        {
            try
            {
                var deviceId = PlayerPrefs.GetString("deviceId", SystemInfo.deviceUniqueIdentifier);

                if (deviceId == SystemInfo.unsupportedIdentifier)
                {
                    deviceId = System.Guid.NewGuid().ToString();
                }

                PlayerPrefs.SetString("deviceId", deviceId);


                _session = await _client.AuthenticateDeviceAsync(deviceId);
                Debug.Log("Authenticated with Device ID");
            }
            catch (ApiResponseException e)
            {
                Debug.LogFormat("Error authenticating with Device ID: {0}", e.Message);
            }
        }

        private async void CreateSocket()
        {
            try
            {
                _socket = _client.NewSocket(useMainThread: true);
                await _socket.ConnectAsync(_session, true);
                _socket.ReceivedChannelMessage += HandleMessageReceived;
            }
            catch (Exception e)
            {
                Debug.LogFormat("Error creating socket with Device ID: {0}", e.Message);
            }
        }

        private async void GetAccount()
        {
            try
            {
                _apiAccount = await _client.GetAccountAsync(_session);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error getting account {e.Message}");
            }
        }
        
        private void HandleMessageReceived(IApiChannelMessage message)
        {
            OnNewMessageReceived?.Invoke(message.Content);
        }

        #endregion

        #region API

        public async void CreateGroup(string groupName, string groupDescription)
        {
            try
            {
                _apiGroup = await _client.CreateGroupAsync(_session, groupName, groupDescription);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error creating group: {e.Message}");
            }
        }

        public async void JoinGroup(string groupID)
        {
            try
            {
                await _client.JoinGroupAsync(_session, groupID);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error joining group: {e.Message}");
            }
        }

        public async void JoinChat()
        {
            try
            {
                _channel = await _socket.JoinChatAsync(_apiGroup.Id, ChannelType.Group, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error joining chat: {e.Message}");
            }
        }

        public async void SendMessage(string message)
        {
            try
            {
                var content = new Dictionary<string, string> { { "message", message } }.ToJson();
                var ack = await _socket.WriteChatMessageAsync(_channel.Id, content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
    }
}