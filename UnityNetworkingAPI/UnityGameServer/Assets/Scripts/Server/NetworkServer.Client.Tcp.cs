using System;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        public partial class Client
        {
            /// <summary>
            ///     Wrapper for the client TCP implementation in the server side.
            /// </summary>
            public class Tcp
            {
                /// <summary>
                ///     Server network configurations.
                /// </summary>
                readonly ServerConfigs _configs;

                /// <summary>
                ///     Data array used by the system to write the received bytes.
                /// </summary>
                byte[] _receiveBuffer;

                /// <summary>
                ///     The last packet received.
                /// </summary>
                Packet _receivedPacket;

                /// <summary>
                ///     The Tcp implementation.
                /// </summary>
                TcpClient _tcpClient;

                /// <summary>
                ///     Private Constructor.
                /// </summary>
                Tcp()
                {
                }

                /// <summary>
                ///    Creates a TCP connection with the client.
                /// </summary>
                internal Tcp(int id, ServerConfigs configs)
                {
                    Id = id;
                    _configs = configs;
                    _receiveBuffer = new byte[BufferSize];
                }

                /// <summary>
                ///     Id of the client.
                /// </summary>
                int Id { get; }

                /// <summary>
                ///     Size of the buffers.
                /// </summary>
                int BufferSize => _configs.BufferSize;

                /// <summary>
                ///     Binds the client to a TCP connection and begins to read the packets.
                /// </summary>
                internal void Connect(TcpClient tcpClient)
                {
                    _tcpClient = tcpClient;
                    Debug.Log($"{tcpClient.Client.RemoteEndPoint} connected successfully id is: {Id}.");
                    _tcpClient.SendBufferSize = BufferSize;
                    _tcpClient.ReceiveBufferSize = BufferSize;
                    GetStream().BeginRead(_receiveBuffer, 0, BufferSize, OnReceiveData, null);
                }

                /// <summary>
                ///     Unbinds the client from the TCP connection, disposes the connection and reset all values.
                /// </summary>
                internal void Disconnect()
                {
                    _tcpClient.Close();
                    _tcpClient.Dispose();
                    _receivedPacket = null;
                    _receiveBuffer = null;
                    _tcpClient = null;
                }

                /// <summary>
                ///     Byte stream from the server.
                /// </summary>
                NetworkStream GetStream()
                {
                    return _tcpClient.GetStream();
                }

                /// <summary>
                ///     Sends the packet to the client using TCP.
                /// </summary>
                internal void SendData(Packet packet)
                {
                    GetStream().BeginWrite(packet.ToArray(), 0, packet.Length, null, null);
                }

                /// <summary>
                ///     Callback received to handle the streaming of bytes from the client.
                /// </summary>
                void OnReceiveData(IAsyncResult result)
                {
                    try
                    {
                        var byteLength = GetStream().EndRead(result);
                        if (byteLength <= 0)
                        {
                            _clientRegistry.GetClient(Id).Disconnect();
                            return;
                        }

                        var data = new byte[byteLength];
                        Array.Copy(_receiveBuffer, data, byteLength);

                        ThreadManager.Schedule(() => HandleData(data));
                        GetStream().BeginRead(_receiveBuffer, 0, BufferSize, OnReceiveData, null);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error receiving TCP data: {ex}");
                        _clientRegistry.GetClient(Id).Disconnect();
                    }
                }

                /// <summary>
                ///     Handles the received data steam.
                /// </summary>
                void HandleData(byte[] data)
                {
                    var packetLength = 0;
                    _receivedPacket = new Packet(data);
                    if (_receivedPacket.UnreadLength >= 4)
                    {
                        packetLength = _receivedPacket.ReadInt();
                        if (packetLength <= 0)
                            return;
                    }

                    while (packetLength > 0 && packetLength <= _receivedPacket.UnreadLength)
                    {
                        var packetBytes = _receivedPacket.ReadBytes(packetLength);
                        var packet = new Packet(packetBytes);
                        var packetId = packet.ReadInt();
                        packet.SetId((PacketId) packetId);

                        packet.Print();

                        var client = _clientRegistry.GetClient(Id);
                        ThreadManager.Schedule(() => client.HandlePacket((PacketId) packetId, packet));

                        packetLength = 0;
                        if (_receivedPacket.UnreadLength < 4)
                            continue;

                        packetLength = _receivedPacket.ReadInt();
                        if (packetLength <= 0)
                            return;
                    }
                }
            }
        }
    }
}