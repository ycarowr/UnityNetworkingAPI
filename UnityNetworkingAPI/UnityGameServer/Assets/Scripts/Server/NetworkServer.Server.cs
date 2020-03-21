using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     Wrapper for the TCP and UDP server connections.
        /// </summary>
        partial class Server
        {
            /// <summary>
            ///     Server configurations.
            /// </summary>
            readonly ServerConfigs _configs;

            /// <summary>
            ///     TCP server connection.
            /// </summary>
            readonly Tcp _tcp;

            /// <summary>
            ///     UDP server connection.
            /// </summary>
            readonly Udp _udp;

            /// <summary>
            ///     Private constructor.
            /// </summary>
            Server()
            {
            }

            /// <summary>
            ///     Creates the server connections.
            /// </summary>
            internal Server(ServerConfigs configs)
            {
                _configs = configs;
                _tcp = new Tcp(_configs);
                _udp = new Udp(_configs);
            }

            /// <summary>
            ///     Whether the server is connected or not.
            /// </summary>
            internal bool IsConnected { get; private set; }

            /// <summary>
            ///     Connects the server to network and enables it receive clients.
            /// </summary>
            internal void Connect()
            {
                if (IsConnected)
                    return;

                _tcp.Connect();
                // _udp.Connect();
                IsConnected = true;
            }

            /// <summary>
            ///     Disconnects the server from the network.
            /// </summary>
            internal void Disconnect()
            {
                if (!IsConnected)
                    return;

                _tcp.Disconnect();
                _udp.Disconnect();
                IsConnected = false;
            }

            /// <summary>
            ///     Sends UDP data to a client.
            /// </summary>
            internal void SendUdpData(int id, Packet packet)
            {
                if (!IsConnected)
                    return;

                packet.InsertId();
                _udp.SendData(id, packet);
            }

            /// <summary>
            ///     Sends TCP data to a client.
            /// </summary>
            internal void SendTcpData(int id, Packet packet)
            {
                if (!IsConnected)
                    return;

                packet.InsertId();
                packet.WriteLength();
                var client = _clientRegistry.GetClient(id);
                client.TcpConnection.SendData(packet);
            }
        }
    }
}