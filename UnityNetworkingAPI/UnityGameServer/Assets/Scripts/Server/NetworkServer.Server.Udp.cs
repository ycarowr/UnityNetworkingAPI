using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     Sends a packet to a client via Udp.
        /// </summary>
        public static void SendUdpData(int clientId, Packet packet)
        {
            _server.SendUdpData(clientId, packet);
        }

        /// <summary>
        ///     Sends a packet to all clients via Udp.
        /// </summary>
        public static void SendUdpDataToAll(Packet packet)
        {
            var clients = _clientRegistry.GetClients();
            foreach (var i in clients)
                _server.SendUdpData(i.Id, packet);
        }

        /// <summary>
        ///     Sends a packet to all clients except one via Udp.
        /// </summary>
        public static void SendUdpDataToAll(int exception, Packet packet)
        {
            var clients = _clientRegistry.GetClients();
            foreach (var i in clients)
                if (i.Id != exception)
                    _server.SendUdpData(i.Id, packet);
        }

        partial class Server
        {
            /// <summary>
            ///     Wrapper for the UDP server connection.
            /// </summary>
            class Udp
            {
                /// <summary>
                ///     The network configurations.
                /// </summary>
                readonly ServerConfigs _configs;
                
                /// <summary>
                ///     The UDP listener.
                /// </summary>
                readonly UdpClient _udpListener;

                /// <summary>
                ///     Private Constructor.
                /// </summary>
                Udp()
                {
                }

                /// <summary>
                ///     Creates a UDP server connection with the specific configs.
                /// </summary>
                internal Udp(ServerConfigs configs)
                {
                    _configs = configs;
                    _udpListener = new UdpClient(_configs.Port);
                    Connect();
                    Debug.Log("Begin UDP listening into: "+_udpListener.Client.LocalEndPoint);
                }

                /// <summary>
                ///     Connects the server with the network allowing clients to connect via UDP.
                /// </summary>
                void Connect()
                {
                    _udpListener.BeginReceive(OnConnect, null);
                }

                /// <summary>
                ///     Disconnects UDP server from the network. Disconnects all clients.
                /// </summary>
                internal void Disconnect()
                {
                    Debug.Log("Disconnect UDP");
                    var clients = _clientRegistry.GetClients();
                    foreach (var client in clients) 
                        client.Disconnect();
                    _udpListener.Close();
                }

                
                void OnConnect(IAsyncResult result)
                {
                    try
                    {
                        var ipEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        var data = _udpListener.EndReceive(result, ref ipEndPoint);
                        _udpListener.BeginReceive(OnConnect, null);
                                                
                        var packet = new Packet(data);
                        var packetLength = packet.ReadInt();
                        var clientId = packet.ReadInt();
                        var client = _clientRegistry.GetClient(clientId);
                        var clientUdpProtocol = client.UdpConnection;
                        var packetId = (PacketId) packet.ReadInt();
                        packet.SetId(packetId);

                        if (!clientUdpProtocol.IsConnected)
                        {
                            client.UdpConnection.ConnectUdp(ipEndPoint);
                            return;
                        }
                        
                        var isSameClient = AssertConnection(clientUdpProtocol.IpEndPoint, ipEndPoint);
                        if (isSameClient)
                            clientUdpProtocol.HandleData(new Packet(data));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error receiving UDP data: {ex}");
                    }
                }

                static bool AssertConnection(IPEndPoint a, IPEndPoint b)
                {
                    return string.Equals(a.ToString(), b.ToString());
                }


                internal void SendData(int id, Packet packet)
                {
                    packet.WriteLength();
                    var client = _clientRegistry.GetClient(id);
                    var ipEndPoint = client.UdpConnection.IpEndPoint;
                    SendUdpDataInternal(ipEndPoint, packet);
                }

                void SendUdpDataInternal(IPEndPoint ipEndPoint, Packet packet)
                {
                    try
                    {
                        if (ipEndPoint != null)
                        {
                            var dataArray = packet.ToArray();
                            var packetLength = packet.Length;
                            AsyncCallback callback = null;
                            object state = null;
                            _udpListener.BeginSend(dataArray, packetLength, ipEndPoint, callback, state);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error sending data to {ipEndPoint} via UDP: {ex}");
                    }
                }
            }
        }
    }
}