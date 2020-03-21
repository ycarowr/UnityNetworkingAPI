using System;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameClient
{
    public static partial class NetworkClient
    {
        partial class Client
        {
            /// <summary>
            ///     Wrapper for the TCP implementation in the client side.
            /// </summary>
            class Tcp
            {
                /// <summary>
                ///     Network configurations.
                /// </summary>
                readonly ClientConfigs _configs;

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
                ///     Creates a TCP with the client.
                /// </summary>
                public Tcp(ClientConfigs configs)
                {
                    _configs = configs;
                    _receiveBuffer = new byte[BufferSize];
                }

                /// <summary>
                ///     Size of the buffers.
                /// </summary>
                int BufferSize => _configs.BufferSize;

                /// <summary>
                ///     Ip from the server.
                /// </summary>
                string Ip => _configs.Ip;

                /// <summary>
                ///     Port used to connect.
                /// </summary>
                int Port => _configs.Port;

                /// <summary>
                ///     The Tcp implementation.
                /// </summary>
                public TcpClient TcpClient => _tcpClient;

                /// <summary>
                ///     Byte stream from the server.
                /// </summary>
                NetworkStream GetStream()
                {
                    return TcpClient.GetStream();
                }

                /// <summary>
                ///     Whether the TCP client is connected with the server or not.
                /// </summary>
                public bool IsConnected()
                {
                    if (TcpClient == null)
                        return false;
                    var isConnected = TcpClient.Client != null && TcpClient.Client.Connected;
                    return isConnected;
                }

                /// <summary>
                ///     Requests the tcp connection with the server.
                /// </summary>
                public void Connect()
                {
                    _tcpClient = new TcpClient
                    {
                        ReceiveBufferSize = BufferSize,
                        SendBufferSize = BufferSize
                    };
                    TcpClient.BeginConnect(Ip, Port, OnConnect, TcpClient);
                }

                /// <summary>
                ///     Disconnects with the server.
                /// </summary>
                public void Disconnect()
                {
                    TcpClient.Close();
                    TcpClient.Dispose();
                    _tcpClient = null;
                    _receiveBuffer = null;
                    _receivedPacket = null;
                    OnDisconnect.Invoke();
                }

                /// <summary>
                ///     Callback received when a TCP connection is established.
                /// </summary>
                void OnConnect(IAsyncResult result)
                {
                    TcpClient.EndConnect(result);
                    if (!TcpClient.Connected)
                        return;

                    object nullState = null;
                    GetStream().BeginRead(_receiveBuffer, 0, BufferSize, OnReceiveData, nullState);
                }

                /// <summary>
                ///     Callback received to handle the streaming of bytes from the server.
                /// </summary>
                void OnReceiveData(IAsyncResult result)
                {
                    var length = GetStream().EndRead(result);
                    if (length <= 0)
                    {
                        Disconnect();
                        return;
                    }

                    var data = new byte[length];
                    Array.Copy(_receiveBuffer, data, length);
                    object nullState = null;
                    ThreadManager.Schedule(() => HandleData(data));
                    GetStream().BeginRead(_receiveBuffer, 0, BufferSize, OnReceiveData, nullState);
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
                        OnHandleData(packetBytes);
                        packetLength = 0;
                        if (_receivedPacket.UnreadLength < 4)
                            continue;
                        packetLength = _receivedPacket.ReadInt();
                        if (packetLength <= 0)
                            return;
                    }
                }

                /// <summary>
                ///     Sends the packet to the server using TCP.
                /// </summary>
                public void SendData(Packet packet)
                {
                    try
                    {
                        if (TcpClient == null)
                            return;

                        var data = packet.ToArray();
                        var offset = 0;
                        var packetLength = packet.Length;
                        object nullState = null;
                        AsyncCallback nullCallback = null;
                        GetStream().BeginWrite(data, offset, packetLength, nullCallback, nullState);
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Error sending data to server via TCP: {ex}");
                    }
                }
            }
        }
    }
}