using UnityEngine;

namespace UnityGameClient
{
    /// <summary>
    ///     Base for any given client application.
    /// </summary>
    public abstract class BaseNetworkApplication : MonoBehaviour
    {
        /// <summary>
        ///     Network configurations. Used to initialize the network system.
        /// </summary>
        [SerializeField] ClientConfigs clientConfigs;

        [Tooltip("Whether the client tries to connect On Awake unity callback."), SerializeField]
        bool connectOnAwake = true;

        /// <summary>
        ///     Whether the application is initialized or not.
        /// </summary>
        public bool IsInitialized { get; private set; }

        void SubscribeClientEvents()
        {
            NetworkClient.OnConnect += OnConnect;
            NetworkClient.OnDisconnect += OnDisconnect;
            NetworkClient.OnReceivePacket += OnReceivePacket;
        }

        void UnsubscribeClientEvents()
        {
            NetworkClient.OnConnect -= OnConnect;
            NetworkClient.OnDisconnect -= OnDisconnect;
            NetworkClient.OnReceivePacket -= OnReceivePacket;
        }

        protected virtual void OnDisconnect()
        {
        }

        protected virtual void OnConnect()
        {
        }

        protected virtual void OnReceivePacket(Packet packet)
        {
        }

        #region Unity Callbacks

        protected virtual void Awake()
        {
            Initialize();
        }

        void OnDestroy()
        {
            Disconnect();
        }

        void OnApplicationQuit()
        {
            Disconnect();
        }

        #endregion

        #region Initialization

        void Initialize()
        {
            NetworkClient.Initialize(clientConfigs);
            IsInitialized = true;
            OnInitialize();
            if (connectOnAwake)
                RequestConnection();
        }

        /// <summary>
        ///     Do you initialization within this method. Executes after the network initialization.
        /// </summary>
        protected virtual void OnInitialize()
        {
            //Do your initialization here...
        }

        #endregion

        #region Operations

        /// <summary>
        ///     Requests the connection to the server.
        /// </summary>
        [Button]
        protected void RequestConnection()
        {
            SubscribeClientEvents();
            NetworkClient.Connect();
        }

        /// <summary>
        ///     Disconnects with the server.
        /// </summary>
        [Button]
        protected void Disconnect()
        {
            UnsubscribeClientEvents();
            NetworkClient.Disconnect();
        }

        #endregion
    }
}