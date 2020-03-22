using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     Sends a packet to a client via Tcp.
        /// </summary>
        internal static void SendTcpData(int clientId, Packet packet)
        {
            _server.SendTcpData(clientId, packet);
        }

        /// <summary>
        ///     Sends a packet to all clients via Tcp.
        /// </summary>
        internal static void SendTcpDataToAll(Packet packet)
        {
            var clients = _clientRegistry.GetClients();
            foreach (var i in clients)
                _server.SendTcpData(i.Id, packet);
        }

        /// <summary>
        ///     Sends a packet to all clients except one via Tcp.
        /// </summary>
        internal static void SendTcpDataToAll(int exception, Packet packet)
        {
            var clients = _clientRegistry.GetClients();
            foreach (var i in clients)
                if (i.Id != exception)
                    _server.SendTcpData(i.Id, packet);
        }

        partial class Server
        {
            /// <summary>
            ///     Wrapper for the TCP server connection.
            /// </summary>
            class Tcp
            {
                /// <summary>
                ///     The network configurations.
                /// </summary>
                readonly ServerConfigs _configs;

                /// <summary>
                ///     The TCP listener.
                /// </summary>
                readonly TcpListener _tcpListener;

                /// <summary>
                ///     Private Constructor.
                /// </summary>
                Tcp()
                {
                }

                /// <summary>
                ///     Creates a listener in a IP address for the specified configurated port.
                /// </summary>
                public Tcp(ServerConfigs configs)
                {
                    _configs = configs;
                    _tcpListener = new TcpListener(IPAddress.Any, _configs.Port);
                }

                /// <summary>
                ///     Connects the TCP listener to the network and enables a client to connect.
                /// </summary>
                internal void Connect()
                {
                    Logger.Log($"Begin TCP listening at address: {_tcpListener.LocalEndpoint}", Color.black, "Server.Tcp");
                    _tcpListener.Start();
                    _tcpListener.BeginAcceptTcpClient(OnConnect, null);
                }

                /// <summary>
                ///     Disconnects the listener from the network.
                /// </summary>
                internal void Disconnect()
                {
                    var clients = _clientRegistry.GetClients();
                    foreach (var client in clients) 
                        client.Disconnect();
                    _tcpListener.Stop();
                }

                /// <summary>
                ///     Callback received when a new connection is established.
                /// </summary>
                void OnConnect(IAsyncResult result)
                {
                    object nullState = null;
                    var tcpClient = _tcpListener.EndAcceptTcpClient(result);
                    _tcpListener.BeginAcceptTcpClient(OnConnect, nullState);

                    if (NumberOfConnections < LimitOfConnections)
                        Client.Welcome(tcpClient, Configs);
                    else
                        Debug.Log(
                            $"Connection with client {tcpClient.Client.RemoteEndPoint}" +
                            " has been denied. Server Full.");
                }
            }
        }
    }
}