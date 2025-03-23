using System;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

namespace Net
{
    public class Client : MonoSingleton<Client>
    {
        public NetworkDriver driver;
        private NetworkConnection connection;

        private bool _isActive;
        private const float KeepAliveInterval = 20.0f;
        private float _lastKeepAlive;

        public event Action ConnectionDroppedEvent;

        #region Base Logic

        public void Initialize(string ip, ushort port)
        {
            driver = NetworkDriver.Create();
            NetworkEndpoint endpoint = NetworkEndpoint.Parse(ip, port);
            connection = driver.Connect(endpoint);
            _isActive = true;
            RegisterToEvent();
        }

        public void Shutdown()
        {
            if (_isActive)
            {
                UnregisterToEvent();
                driver.Dispose();
                _isActive = false;
                connection = default(NetworkConnection);
            }
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void RegisterToEvent()
        {
            NetUtility.C_KEEP_ALIVE += OnKeepAlive;
        }


        private void UnregisterToEvent()
        {
            NetUtility.C_KEEP_ALIVE -= OnKeepAlive;
        }

        private void OnKeepAlive(NetMessage.NetMessage obj)
        {
        }

        #endregion

        private void Update()
        {
            if (!_isActive) return;

            driver.ScheduleUpdate().Complete();
            CheckAlive();
            UpdateMessagePump();
        }

        void CheckAlive()
        {
            if (!connection.IsCreated && _isActive)
            {
                Debug.LogWarning("Something went wrong, lost connection to the server.");
                ConnectionDroppedEvent?.Invoke();
                Shutdown();
            }
        }

        void UpdateMessagePump()
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Connect)
                {
                    //SendToServer(new NetWelcome());
                    Debug.Log("We're connected to the server.");
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, default(NetworkConnection));
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.LogWarning("Client disconnected from server");
                    connection = default(NetworkConnection);
                    ConnectionDroppedEvent?.Invoke();
                    Shutdown();
                }
            }
        }

        public void SendToServer(NetMessage.NetMessage message)
        {
            DataStreamWriter writer;
            driver.BeginSend(connection, out writer);
            message.Serialize(ref writer);
            driver.EndSend(writer);
        }
    }
}