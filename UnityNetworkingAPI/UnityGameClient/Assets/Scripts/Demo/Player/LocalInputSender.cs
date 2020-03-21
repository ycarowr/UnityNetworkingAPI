using UnityEngine;
using UnityGameClient;

namespace Demo
{
    /// <summary>
    ///     Grabs the local input ands sends it to the server.
    /// </summary>
    public class LocalInputSender : MonoBehaviour
    {
        void FixedUpdate()
        {
            SendInputToServer();
        }

        void SendInputToServer()
        {
            var inputs = new[]
            {
                Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.W),
                Input.GetKey(KeyCode.S),
                Input.GetKey(KeyCode.Space)
            };

            SendPlayerMovement(inputs, transform.rotation);
        }

        /// <summary>
        ///     Sends the player movement to be calculated by the server.
        /// </summary>
        void SendPlayerMovement(bool[] inputs, Quaternion rotation)
        {
            var packet = new Packet(PacketId.PlayerMovement);
            packet.Write(inputs.Length);
            foreach (var input in inputs)
                packet.Write(input);
            packet.Write(rotation);
            this.SendUdpData(packet);
        }
    }
}