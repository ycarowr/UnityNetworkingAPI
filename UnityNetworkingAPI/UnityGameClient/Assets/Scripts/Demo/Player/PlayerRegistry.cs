using System.Collections.Generic;
using UnityEngine;
using UnityGameClient;

namespace Demo
{
    /// <summary>
    ///     Local player register.
    /// </summary>
    public class PlayerRegistry : MonoBehaviour
    {
        /// <summary>
        ///     All instantiated players.
        /// </summary>
        readonly Dictionary<int, PlayerController> _players = new Dictionary<int, PlayerController>();

        /// <summary>
        ///     Client network application.
        /// </summary>
        [SerializeField] DemoClientApp clientApp;

        /// <summary>
        ///     Player Prefab.
        /// </summary>
        [SerializeField] PlayerController playerPrefab;


        /// <summary>
        /// </summary>
        void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
        {
            var player = Instantiate(playerPrefab);
            player.Initialize(id, username, position, rotation);

            Debug.Log(GetType() + " Spawn Player, is local client: " + player.IsLocalClient);
            _players.Add(id, player);

            //Adds the input component for the local player.
            if (!player.IsLocalClient)
                return;

            player.gameObject.AddComponent<LocalInputSender>();
        }

        void TrySpawnPlayer(Packet packet)
        {
            if (packet.Id != PacketId.SpawnPlayer)
                return;

            var id = packet.ReadInt();
            var username = packet.ReadString();
            var position = packet.ReadVector3();
            var rotation = packet.ReadQuaternion();
            SpawnPlayer(id, username, position, rotation);
        }

        #region Initialization

        void Awake()
        {
            SubscribeNetwork();
        }

        void OnDestroy()
        {
            UnsubscribeNetwork();
        }

        void SubscribeNetwork()
        {
            NetworkClient.OnReceivePacket += TrySpawnPlayer;
        }

        void UnsubscribeNetwork()
        {
            NetworkClient.OnReceivePacket -= TrySpawnPlayer;
        }

        #endregion
    }
}