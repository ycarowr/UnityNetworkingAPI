using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     Wrapper for TCP and UDP client.
        /// </summary>
        partial class Client : IClient
        {
            const string WelcomeMessage = "Welcome to the server!";

            /// <summary>
            ///     Wrapper for the TCP protocol.
            /// </summary>
            readonly Tcp _tcpConnection;

            /// <summary>
            ///     Wrapper for the UDP protocol.
            /// </summary>
            readonly Udp _udpConnection;

            /// <summary>
            ///     Private Constructor.
            /// </summary>
            Client()
            {
            }

            /// <summary>
            ///     Creates a client with the id.
            /// </summary>
            Client(int id, ServerConfigs configs)
            {
                Id = id;

                _tcpConnection = new Tcp(Id, configs);
                _udpConnection = new Udp(Id, configs);
            }

            /// <summary>
            ///     TCP Client's connection.
            /// </summary>
            internal Tcp TcpConnection => _tcpConnection;

            /// <summary>
            ///     UDP Client's connection.
            /// </summary>
            internal Udp UdpConnection => _udpConnection;

            /// <summary>
            ///     Client unique identification.
            /// </summary>
            public int Id { get; }

            /// <summary>
            ///     Event dispatched when a client sends a package to the server.
            /// </summary>
            public event Action<Packet> OnReceivePacket = packet => { };

            /// <summary>
            ///     Connects the client through TCP.
            /// </summary>
            void ConnectTcp(TcpClient tcpClient)
            {
                _tcpConnection.Connect(tcpClient);
            }

            /// <summary>
            ///     Disconnects the client
            /// </summary>
            internal void Disconnect()
            {
                _tcpConnection.Disconnect();
                _udpConnection.Disconnect();
                _clientRegistry.UnregisterClient(this);
                OnClientDisconnect.Invoke(this);
            }

            /// <summary>
            ///     Welcomes a incoming client's connection to the server. Welcome packets are handled internally.
            /// </summary>
            internal static void Welcome(TcpClient tcpClient, ServerConfigs configs)
            {
                var clientId = IdHelper.GetNextId();
                var client = new Client(clientId, configs);
                if (!_clientRegistry.RegisterClient(client))
                {
                    Debug.LogError("Error registry: client id already exists.");
                    return;
                }

                client.ConnectTcp(tcpClient);
                var packet = new Packet(PacketId.Welcome);
                packet.Write(WelcomeMessage);
                packet.Write(clientId);
                SendTcpData(clientId, packet);
                Debug.Log($"Client Id {clientId} Welcome");
            }

            /// <summary>
            ///     Confirms that a clients received its identification and dispatches a new client event.
            /// </summary>
            void WelcomeResponse(Packet packet)
            {
                var idInPacket = packet.ReadInt();
                var username = packet.ReadString();
                Debug.Log($"Welcome Received for client id: {Id} and username: {username}");
                if (Id == idInPacket)
                {
                    OnClientConnect.Invoke(this);
                }
                else
                {
                    Debug.Log($"(ID: {Id}) has assumed the wrong client ID ({idInPacket})!");
                    Disconnect();
                }
            }

            /// <summary>
            ///     Handles a received packet. Welcome packets are handled internally.
            /// </summary>
            void HandlePacket(PacketId packetId, Packet packet)
            {
                if (packetId == PacketId.WelcomeResponse)
                {
                    WelcomeResponse(packet);
                }
                else
                {
                    //dispatch local client event.
                    OnReceivePacket.Invoke(packet);

                    //dispatch global server event.
                    OnClientReceivePacket(this, packet);
                }
            }

            static class IdHelper
            {
                /// <summary>
                ///     Global current identification.
                /// </summary>
                static int _globalId;

                /// <summary>
                ///     Generates the next unique identification.
                /// </summary>
                public static int GetNextId()
                {
                    return ++_globalId;
                }
            }
        }
    }
}