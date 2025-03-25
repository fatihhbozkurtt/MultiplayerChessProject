using System;
using Net.NetMessage;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net
{
    public class Server : MonoSingleton<Server>
    {
        public NetworkDriver driver;
        private NativeList<NetworkConnection> connections = new NativeList<NetworkConnection>();

        private bool _isActive;
        private const float KeepAliveInterval = 20.0f;
        private float _lastKeepAlive;

        public event Action ConnectionDroppedEvent;

        public void Initialize(ushort port)
        {
            driver = NetworkDriver.Create();
            NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.Port = port;

            if (driver.Bind(endpoint) != 0)
            {
                Debug.LogError($"Unable to bind to port {port}");
                return;
            }

            driver.Listen();
            Debug.Log("Successfully binded to port " + port);

            connections = new NativeList<NetworkConnection>(2, Allocator.Persistent);
            _isActive = true;
        }

        public void Shutdown()
        {
            if (_isActive)
            {
                driver.Dispose();
                connections.Dispose();
                _isActive = false;
            }
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void Update()
        {
            if (!_isActive) return;

            KeepAlive();
            driver.ScheduleUpdate().Complete();
            CleanUpConnections();
            AcceptNewConnections();
            UpdateMessagePump();
        }

        void KeepAlive()
        {
            if (Time.time - _lastKeepAlive > KeepAliveInterval)
            {
                _lastKeepAlive = Time.time;
                Broadcast(new NetKeepAlive());
            }
        }

        void CleanUpConnections()
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].IsCreated)
                {
                    connections.RemoveAtSwapBack(i);
                    --i;
                }
            }
        }

        void AcceptNewConnections()
        {
            NetworkConnection c;

            while ((c = driver.Accept()) != default(NetworkConnection))
            {
                connections.Add(c);
            }
        }

        void UpdateMessagePump()
        {
            DataStreamReader stream;

            for (int i = 0; i < connections.Length; i++)
            {
                NetworkEvent.Type cmd;

                while ((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        NetUtility.OnData(stream, connections[i], this);
                    }
                    else if (cmd == NetworkEvent.Type.Disconnect)
                    {
                        Debug.LogWarning("Client disconnected from server");
                        connections[i] = default(NetworkConnection);
                        ConnectionDroppedEvent?.Invoke();
                        Shutdown();
                    }
                }
            }
        }

        #region Server Specific

        public void SendToClient(NetworkConnection connection, NetMessage.NetMessage message)
        {
            DataStreamWriter writer;
            driver.BeginSend(connection, out writer);
            message.Serialize(ref writer);
            driver.EndSend(writer);
        }

        public void Broadcast(NetMessage.NetMessage message)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].IsCreated)
                {
                    SendToClient(connections[i], message);
                }
            }
        }

        #endregion
    }
}