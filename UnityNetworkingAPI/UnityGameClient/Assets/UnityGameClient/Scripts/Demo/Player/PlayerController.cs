using UnityEngine;
using UnityGameClient;

namespace Demo
{
    /// <summary>
    ///     Holds information about a player.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        readonly Color _localPlayerColor = Color.blue;
        readonly Color _networkPlayerColor = Color.yellow;
        Renderer _renderer;
        public int Id { get; private set; }
        public string UserName { get; private set; }
        public bool IsLocalClient => NetworkClient.Id == Id;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void Initialize(int id, string userName, Vector3 position, Quaternion rotation)
        {
            Id = id;
            UserName = userName;
            SetPosition(position);
            SetRotation(rotation);
            SetColor();
            NetworkClient.OnReceivePacket += HandleRotation;
            NetworkClient.OnReceivePacket += HandleMovement;
        }

        void HandleMovement(Packet packet)
        {
            var bytes = packet.ToArray();
            var localPacket = new Packet(bytes);
            localPacket.SetId((PacketId) localPacket.ReadInt());
            if (packet.Id != PacketId.PlayerPosition)
                return;
            var id = localPacket.ReadInt();

            if (id != Id)
                return;
            var position = localPacket.ReadVector3();
            transform.position = position;
        }

        void HandleRotation(Packet packet)
        {
            var bytes = packet.ToArray();
            var localPacket = new Packet(bytes);
            localPacket.SetId((PacketId) localPacket.ReadInt());
            if (packet.Id != PacketId.PlayerRotation)
                return;

            var id = localPacket.ReadInt();

            if (id != Id)
                return;

            var rotation = localPacket.ReadQuaternion();
            transform.rotation = rotation;
        }

        void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        void SetColor()
        {
            _renderer.material.color = IsLocalClient ? _localPlayerColor : _networkPlayerColor;
        }

        void OnDestroy()
        {
            NetworkClient.OnReceivePacket -= HandleRotation;
            NetworkClient.OnReceivePacket -= HandleMovement;
        }
    }
}