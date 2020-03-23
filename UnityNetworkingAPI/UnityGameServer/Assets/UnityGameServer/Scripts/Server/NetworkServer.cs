using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     Wrapper for TCP and UDP protocols.
        /// </summary>
        static Server _server;

        /// <summary>
        ///     The client registry.
        /// </summary>
        static ClientRegistry _clientRegistry;

        /// <summary>
        ///     Server's network configurations.
        /// </summary>
        public static ServerConfigs Configs { get; private set; }

        /// <summary>
        ///     Port which the server is connected.
        /// </summary>
        public static int Port => Configs.Port;

        /// <summary>
        ///     Amount of clients currently connected to the server.
        /// </summary>
        public static int NumberOfConnections => _clientRegistry.ClientCount;

        /// <summary>
        ///     Amount maximum of clients connections the server can handle at the same time.
        /// </summary>
        public static int LimitOfConnections => Configs.LimitOfConnections;

        /// <summary>
        ///     Whether the server is initialized or not.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        ///     Whether the server is connected or not.
        /// </summary>
        public static bool IsConnected => _server.IsConnected;

        /// <summary>
        ///     Event dispatched when a client is connected to the server.
        /// </summary>
        public static event Action<IClient> OnClientConnect = client => { };

        /// <summary>
        ///     Event dispatched when a client is disconnected from the server.
        /// </summary>
        public static event Action<IClient> OnClientDisconnect = client => { };

        /// <summary>
        ///     Event dispatched when a client receive a packet.
        /// </summary>
        public static event Action<IClient, Packet> OnClientReceivePacket = (client, packet) => { };

        #region Initialization

        /// <summary>
        ///     Initializes the server system with the configurations.
        /// </summary>
        public static void Initialize(ServerConfigs configs)
        {
            if (IsInitialized)
                return;

            Logger.Log("Initializing Server ...", Color.blue, "NetworkServer");

            SetupConfigurations(configs);
            InitializeClientRegistry();
            InitializeServerConnections();

            Logger.Log($"Server started on port {Port}.", Color.black, "NetworkServer");
            IsInitialized = true;
        }

        /// <summary>
        ///     Sets up all the configs in order to work properly.
        /// </summary>
        static void SetupConfigurations(ServerConfigs configs)
        {
            Logger.Log("Setting up configurations ...", Color.blue, "NetworkServer");
            Configs = configs;
            QualitySettings.vSyncCount = Configs.ServerVSyncDefault;
            Application.targetFrameRate = Configs.ServerFrameRate;
            Logger.Log("Configurations set successfully.", Color.black, "NetworkServer");
        }

        /// <summary>
        ///     Initializes the client registry with no client.
        /// </summary>
        static void InitializeClientRegistry()
        {
            Logger.Log("Initializing Client Registry ...", Color.blue, "NetworkServer");
            _clientRegistry = new ClientRegistry();
            Logger.Log("Client Registry Successfully Initialized.", Color.black, "NetworkServer");
        }

        /// <summary>
        ///     Initializes the connection protocols.
        /// </summary>
        static void InitializeServerConnections()
        {
            Logger.Log("Initializing Server Connections ...", Color.blue, "NetworkServer");
            _server = new Server(Configs);
            Logger.Log("Server Connections Initialized.", Color.black, "NetworkServer");
        }

        #endregion

        #region Operations

        /// <summary>
        ///     Disconnects the server and terminates all the connections with the clients.
        /// </summary>
        public static void Disconnect()
        {
            if (!IsInitialized)
                return;

            var clients = _clientRegistry.GetClients();
            foreach (var client in clients)
                client.Disconnect();

            _server.Disconnect();
            _server = null;
            IsInitialized = false;
            Logger.Log("Server Disconnected", Color.red, "NetworkServer");
        }

        /// <summary>
        ///     Connects the server to the network allowing clients to request connections.
        /// </summary>
        public static void Connect()
        {
            if (!IsInitialized)
                return;

            _server.Connect();
            Logger.Log("Server Connected!", Color.black, "NetworkServer");
        }

        /// <summary>
        ///     Returns the client with the specified id. Null if not initialized or not found.
        /// </summary>
        public static IClient GetClient(int id)
        {
            if (!IsInitialized)
                return null;

            return _clientRegistry.GetClient(id);
        }

        /// <summary>
        ///     Returns all registered clients. Null if not initialized.
        /// </summary>
        public static IClient[] GetClients()
        {
            if (!IsInitialized)
                return null;

            return _clientRegistry.GetClients();
        }

        #endregion
    }
}