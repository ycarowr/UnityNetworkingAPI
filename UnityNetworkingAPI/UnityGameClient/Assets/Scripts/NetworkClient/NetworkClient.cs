using System;
using UnityEngine;

namespace UnityGameClient
{
    public static partial class NetworkClient
    {
        /// <summary>
        ///     Wrapper for the protocols functions. TCP and UDP.
        /// </summary>
        static Client _client;

        /// <summary>
        ///     Networking configurations.
        /// </summary>
        public static ClientConfigs Configs { get; private set; }

        /// <summary>
        ///     Id of this client application.
        /// </summary>
        public static int Id => _client.Id;

        /// <summary>
        ///     Whether the networking system initialized or not.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Dispatched whenever the client connects thought tcp with the server.
        /// </summary>
        public static event Action OnConnect = () => { };

        /// <summary>
        ///     Dispatched whenever disconnected.
        /// </summary>
        public static event Action OnDisconnect = () => { };

        /// <summary>
        ///     Dispatched whenever received a packet. Either tcp or udp.
        /// </summary>
        public static event Action<Packet> OnReceivePacket = packet => { };

        /// <summary>
        ///     Initialize the networking system with the configurations.
        /// </summary>
        public static void Initialize(ClientConfigs configs)
        {
            if (IsInitialized)
                return;

            Debug.Log("Initialize Networking System");
            Configs = configs;
            _client = new Client(Configs);
            IsInitialized = true;
        }

        /// <summary>
        ///     Requests connection with the server.
        /// </summary>
        public static void Connect()
        {
            if (!IsInitialized)
                return;
            Debug.Log("Client Requested Connection ...");
            if (_client == null)
                _client = new Client(Configs);

            _client.ConnectTcp();
        }

        /// <summary>
        ///     Disconnects with the server.
        /// </summary>
        public static void Disconnect()
        {
            if (!IsInitialized)
                return;

            _client?.Disconnect();
            _client = null;
            Debug.Log("Client Disconnected");
        }

        /// <summary>
        ///     Sends a packet using the TCP protocol.
        /// </summary>
        public static void SendTcpData(Packet packet)
        {
            if (!IsInitialized)
                return;

            packet.InsertId();
            packet.WriteLength();
            _client.SendTcpData(packet);
        }

        /// <summary>
        ///     Sends a packet using the UDP protocol.
        /// </summary>
        public static void SendUdpData(Packet packet)
        {
            if (!IsInitialized)
                return;
            packet.InsertId();
            packet.InsertInt(Id);
            packet.WriteLength();
            _client.SendUdpData(packet);
        }

        /// <summary>
        ///     Receives a package and handles the welcome package. Welcome packets are handled internally.
        /// </summary>
        static void OnHandleData(byte[] data)
        {
            var packet = new Packet(data);
            var packetId = (PacketId) packet.ReadInt();
            packet.SetId(packetId);

            if (packetId == PacketId.Welcome)
                OnWelcome(packet);
            else
                OnReceivePacket.Invoke(packet);
        }

        /// <summary>
        ///     Responds the server acknowledging the received id.
        /// </summary>
        static void OnWelcome(Packet packet)
        {
            var msg = packet.ReadString();
            var id = packet.ReadInt();

            _client.SetId(id);

            var welcomeResponse = new Packet(PacketId.WelcomeResponse);
            welcomeResponse.Write(id);
            welcomeResponse.Write(Configs.UserName);
            SendTcpData(welcomeResponse);
            welcomeResponse.Dispose();
            Debug.Log("Attempt UDP Connection ...");
            _client.ConnectUdp();
            OnConnect.Invoke();
            Debug.Log($"Application Connected: Server Message: {msg}, your id is {id}");
        }
    }
}