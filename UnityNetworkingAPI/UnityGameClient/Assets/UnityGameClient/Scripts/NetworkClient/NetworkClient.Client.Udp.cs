using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameClient
{
    public static partial class NetworkClient
    {
        partial class Client
        {
            /// <summary>
            ///     Wrapper for the UDP implementation in the client side.
            /// </summary>
            class Udp
            {
                /// <summary>
                ///     Network configurations.
                /// </summary>
                readonly ClientConfigs _configs;

                /// <summary>
                ///     The connection Ip.
                /// </summary>
                IPEndPoint _ipEndPoint;

                /// <summary>
                ///     The UDP client socket.
                /// </summary>
                UdpClient _udpClient;

                /// <summary>
                ///     Creates a UDP connection.
                /// </summary>
                public Udp(ClientConfigs configs)
                {
                    _configs = configs;
                    var ip = _configs.Ip;
                    var ipAddress = IPAddress.Parse(ip);
                    _ipEndPoint = new IPEndPoint(ipAddress, _configs.Port);
                }

                /// <summary>
                ///     Attempts to connect to the server.
                /// </summary>
                public void Connect(int port)
                {
                    _udpClient = new UdpClient(port);
                    _udpClient.Connect(_ipEndPoint);
                    _udpClient.BeginReceive(OnReceive, null);
                    var packet = new Packet(PacketId.TestPacket);
                    NetworkClient.SendUdpData(packet);
                }

                /// <summary>
                ///     Sends data to the client via UDP.
                /// </summary>
                public void SendData(Packet packet)
                {
                    if (packet != null) _udpClient?.BeginSend(packet.ToArray(), packet.Length, null, null);
                }

                /// <summary>
                ///     Receives incoming UDP data.
                /// </summary>
                void OnReceive(IAsyncResult result)
                {
                    try
                    {
                        var data = _udpClient.EndReceive(result, ref _ipEndPoint);
                        _udpClient.BeginReceive(OnReceive, null);
                        var packetLength = 0;
                        var receivedPacket = new Packet(data);
                        if (receivedPacket.UnreadLength >= 4)
                        {
                            packetLength = receivedPacket.ReadInt();
                            if (packetLength <= 0)
                                return;
                        }

                        while (packetLength > 0 && packetLength <= receivedPacket.UnreadLength)
                        {
                            var packetBytes = receivedPacket.ReadBytes(packetLength);
                            ThreadManager.Schedule(() => OnHandleData(packetBytes));
                            packetLength = 0;
                            if (receivedPacket.UnreadLength < 4)
                                continue;
                            packetLength = receivedPacket.ReadInt();
                            if (packetLength <= 0)
                                return;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }

                /// <summary>
                ///     Disconnects from the server and cleans up the UDP connection.
                /// </summary>
                public void Disconnect()
                {
                    Debug.Log("Disconnect UDP");
                    _ipEndPoint = null;
                }
            }
        }
    }
}