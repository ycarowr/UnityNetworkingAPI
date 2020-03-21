using UnityEngine;

namespace UnityGameServer
{
    /// <summary>
    ///     Base class for any given server. It wraps the most common networking functionalities.
    /// </summary>
    public abstract class BaseServerApplication : MonoBehaviour
    {
        /// <summary>
        ///     The server configurations.
        /// </summary>
        [SerializeField] ServerConfigs configs;

        [Tooltip("Whether the server connects on the On Awake callback."), SerializeField]
        bool isTurnOnAwake = true;

        #region Unity Callbacks

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            TurnServerOff();
        }

        protected virtual void OnApplicationQuit()
        {
            TurnServerOff();
        }

        #endregion

        #region Initialization

        /// <summary>
        ///     Initializes the Server Application.
        /// </summary>
        void Initialize()
        {
            NetworkServer.Initialize(configs);
            OnInitialize();

            if (isTurnOnAwake)
                TurnServerOn();
        }

        void SubscribeNetworkEvents()
        {
            NetworkServer.OnClientConnect += OnClientConnect;
            NetworkServer.OnClientDisconnect += OnClientDisconnect;
            NetworkServer.OnClientReceivePacket += OnClientReceivedPacket;
        }

        void UnsubscribeNetworkEvents()
        {
            NetworkServer.OnClientConnect -= OnClientConnect;
            NetworkServer.OnClientDisconnect -= OnClientDisconnect;
            NetworkServer.OnClientReceivePacket -= OnClientReceivedPacket;
        }


        /// <summary>
        ///     Called when a client is connected to the server.
        /// </summary>
        protected virtual void OnClientConnect(NetworkServer.IClient client)
        {
        }

        /// <summary>
        ///     Called when a client is disconnected from the server.
        /// </summary>
        protected virtual void OnClientDisconnect(NetworkServer.IClient client)
        {
        }

        /// <summary>
        ///     Called when a client receives a packet.
        /// </summary>
        protected virtual void OnClientReceivedPacket(NetworkServer.IClient client, Packet packet)
        {
        }

        /// <summary>
        ///     Dispatched when the server app initializes.
        /// </summary>
        protected virtual void OnInitialize()
        {
            //Do your own initialization here.
        }

        #endregion

        #region Operations

        /// <summary>
        ///     Turns on the server.
        /// </summary>
        [Button]
        public void TurnServerOn()
        {
            // in case the server is not yet initialized.
            if (!NetworkServer.IsInitialized)
                NetworkServer.Initialize(configs);

            SubscribeNetworkEvents();
            NetworkServer.Connect();
        }

        /// <summary>
        ///     Turns off the server.
        /// </summary>
        [Button]
        public void TurnServerOff()
        {
            UnsubscribeNetworkEvents();
            NetworkServer.Disconnect();
        }

        #endregion
    }
}