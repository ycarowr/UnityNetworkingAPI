using System.Collections.Generic;
using UnityEngine;
using UnityGameServer;

namespace Demo
{
    /// <summary>
    ///     Local player registry.
    /// </summary>
    public class PlayerRegistry : MonoBehaviour
    {
        /// <summary>
        ///     All instantiated players.
        /// </summary>
        readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();

        Vector3 _distanceBetweenPlayerSpawn = new Vector3(0, 0, 1);

        /// <summary>
        ///     Player Prefab.
        /// </summary>
        [SerializeField] Player playerPrefab;
        
        void Awake()
        {
            NetworkServer.OnClientConnect += SpawnPlayer;
            NetworkServer.OnClientDisconnect += OnDisconnect;
        }

        void OnDestroy()
        {
            NetworkServer.OnClientConnect -= SpawnPlayer;
            NetworkServer.OnClientDisconnect -= OnDisconnect;
        }

        void SpawnPlayer(NetworkServer.IClient client)
        {
            var position = Vector3.up + _distanceBetweenPlayerSpawn;
            var rotation = Quaternion.identity;
            var userName = string.Empty;
            var id = client.Id;
            var player = Instantiate(playerPrefab);
            _players.Add(id, player);
            player.Initialize(id, userName, position, rotation);
            SendNewPlayerToAllPlayers(player);
            SendAllPlayersToNewPlayer(player);
            _distanceBetweenPlayerSpawn += Vector3.forward;
        }

        /// <summary>
        ///     Sends the new player to all the clients.
        /// </summary>
        void SendNewPlayerToAllPlayers(Player newPlayer)
        {
            foreach (var player in _players.Values)
            {
                var packet = new Packet(PacketId.SpawnPlayer);
                packet.Write(newPlayer.Id);
                packet.Write(newPlayer.UserName);
                packet.Write(newPlayer.transform.position);
                packet.Write(newPlayer.transform.rotation);
                this.SendTcpData(player.Id, packet);
            }
        }

        /// <summary>
        ///     Sends all the clients to the new player. Except himself.
        /// </summary>
        void SendAllPlayersToNewPlayer(Player newPlayer)
        {
            foreach (var player in _players.Values)
                //don't sent itself
                if (player.Id != newPlayer.Id)
                {
                    var packet = new Packet(PacketId.SpawnPlayer);
                    packet.Write(player.Id);
                    packet.Write(player.UserName);
                    packet.Write(player.transform.position);
                    packet.Write(player.transform.rotation);
                    this.SendTcpData(newPlayer.Id, packet);
                }
        }

        /// <summary>
        ///     Handles a client disconnection.
        /// </summary>
        void OnDisconnect(NetworkServer.IClient client)
        {
            var id = client.Id;
            var disconnectedPlayer = _players[id];
            _players.Remove(id);
            ThreadManager.Schedule(() => Destroy(disconnectedPlayer.gameObject));
            //Send destroy
        }
    }
}