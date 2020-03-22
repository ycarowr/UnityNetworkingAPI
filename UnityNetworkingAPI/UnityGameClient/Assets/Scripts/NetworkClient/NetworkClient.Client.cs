using System.Net;
using UnityEngine;

namespace UnityGameClient
{
    public static partial class NetworkClient
    {
        /// <summary>
        ///     Wrapper for TCP and UDP protocols.
        /// </summary>
        partial class Client
        {
            /// <summary>
            ///     Wrapper for the TCP protocol.
            /// </summary>
            readonly Tcp _tcp;

            /// <summary>
            ///     The client id.
            /// </summary>
            int _id;

            /// <summary>
            ///     Wrapper for the UDP protocol.
            /// </summary>
            Udp _udp;

            /// <summary>
            ///     Private constructor.
            /// </summary>
            Client()
            {
            }

            /// <summary>
            ///     Creates a client with the network configs.
            /// </summary>
            public Client(ClientConfigs configs)
            {
                _tcp = new Tcp(configs);
            }

            /// <summary>
            ///     Id of the client.
            /// </summary>
            public int Id => _id;

            /// <summary>
            ///     Connects the client using the TCP protocol.
            /// </summary>
            public void ConnectTcp()
            {
                if (IsConnected())
                    return;
                
                _tcp.Connect();
            }

            /// <summary>
            ///     Connects the client using the UDP protocol.
            /// </summary>
            public void ConnectUdp()
            {
                var tcpClient = _tcp.TcpClient;
                var client = tcpClient.Client;
                var localEndPoint = client.LocalEndPoint;
                var ipEndPoint = (IPEndPoint) localEndPoint;
                var port = ipEndPoint.Port;
                Logger.Log($"Attempt UDP Connection on IP: {ipEndPoint}, Port: {port} ", Color.blue, "Client");
                _udp = new Udp(Configs);
                _udp.Connect(port);
            }

            /// <summary>
            ///     Disconnects the client with the server.
            /// </summary>
            public void Disconnect()
            {
                if (!IsConnected())
                    return;

                _tcp.Disconnect();
                _udp.Disconnect();
            }

            /// <summary>
            ///     Whether the player is connected with the server or not.
            /// </summary>
            public bool IsConnected()
            {
                return _tcp.IsConnected();
            }

            /// <summary>
            ///     Sends a packet using the TCP protocol.
            /// </summary>
            public void SendTcpData(Packet packet)
            {
                _tcp.SendData(packet);
            }

            /// <summary>
            ///     Sends a packet using the UDP protocol.
            /// </summary>
            public void SendUdpData(Packet packet)
            {
                _udp.SendData(packet);
            }

            /// <summary>
            ///     Sets the client identification code.
            /// </summary>
            public void SetId(int id)
            {
                _id = id;
            }
        }
    }
}