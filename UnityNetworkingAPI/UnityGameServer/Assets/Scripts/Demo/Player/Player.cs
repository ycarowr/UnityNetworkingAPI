using System;
using UnityEngine;
using UnityGameServer;

namespace Demo
{
    /// <summary>
    ///     Holds information about a player.
    /// </summary>
    public class Player : MonoBehaviour
    {
        CharacterController _characterController;
        float _gravity = -9.81f;
        bool[] _inputs;
        float _jumpSpeed = 5f;
        float _moveSpeed = 5f;
        float _yVelocity;
        public string UserName { get; private set; }
        public int Id { get; private set; }

        public void Initialize(int id, string userName, Vector3 position, Quaternion rotation)
        {
            Id = id;
            UserName = userName;
            SetPosition(position);
            SetRotation(rotation);
            _gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            _moveSpeed *= Time.fixedDeltaTime;
            _jumpSpeed *= Time.fixedDeltaTime;
            _inputs = new bool[5];
            _characterController = GetComponent<CharacterController>();
            NetworkServer.OnClientReceivePacket += HandleMovement;
        }

        void HandleMovement(NetworkServer.IClient client, Packet packet)
        {
            if (packet.Id == PacketId.PlayerMovement)
            {
                var inputLength = packet.ReadInt();
                var inputs = new bool[inputLength];
                for (var index = 0; index < inputs.Length; index++)
                    inputs[index] = packet.ReadBool();
                var rotation = packet.ReadQuaternion();

                SetInput(inputs, rotation);
            }
        }
        
        void SetInput(bool[] inputs, Quaternion rotation)
        {
            _inputs = inputs;
            transform.rotation = rotation;
        }

        public void FixedUpdate()
        {
            var inputDirection = Vector2.zero;

            if (_inputs[0])
                inputDirection.y += 1;
            if (_inputs[1])
                inputDirection.y -= 1;
            if (_inputs[2])
                inputDirection.x -= 1;
            if (_inputs[3])
                inputDirection.x += 1;

            Move(inputDirection);
        }

        void Move(Vector2 directions)
        {
            var myTransform = transform;
            var moveDirection = -myTransform.right * directions.x + myTransform.forward * directions.y;
            moveDirection *= _moveSpeed;

            if (_characterController.isGrounded)
            {
                _yVelocity = 0f;
                if (_inputs[4])
                    _yVelocity = _jumpSpeed;
            }

            _yVelocity += _gravity;
            moveDirection.y = _yVelocity;
            _characterController.Move(moveDirection);
            SendPlayerPosition();
            SendPlayerRotation();
        }

        void SetRotation(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        void SendPlayerPosition()
        {
            var packet = new Packet(PacketId.PlayerPosition);
            packet.Write(Id);
            packet.Write(transform.position);
            this.SendUdpDataAll(packet);
        }

        void SendPlayerRotation()
        {
            var packet = new Packet(PacketId.PlayerRotation);
            packet.Write(Id);
            packet.Write(transform.rotation);
            this.SendUdpDataAll(packet);
        }

        void OnDestroy()
        {
            NetworkServer.OnClientReceivePacket -= HandleMovement;
        }
    }
}