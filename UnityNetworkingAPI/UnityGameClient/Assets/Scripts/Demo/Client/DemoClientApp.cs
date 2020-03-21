using UnityEngine;
using UnityGameClient;

namespace Demo
{
    /// <summary>
    ///     A simple demo application.
    /// </summary>
    public class DemoClientApp : BaseNetworkApplication
    {
        #region Initialization and Callbacks

        protected override void OnInitialize()
        {
            Debug.Log("Demo Client Initialized!");
        }

        protected override void OnDisconnect()
        {
            Debug.Log("Application has been Disconnected!");
        }

        protected override void OnConnect()
        {
            Debug.Log("Application has been Connected!");
        }

        protected override void OnReceivePacket(Packet packet)
        {
            Debug.Log($"Application has received a packet, id {packet.Id}!");

            //Handling a test package
            if (packet.Id != PacketId.TestPacket)
                return;

            var msg = packet.ReadString();
            Debug.Log($"Server has sent a test message: {msg}");
        }

        #endregion

        #region Test

        [Header("Demo Test"), SerializeField] string testMessage;

        [Button]
        void SendTestMessage()
        {
            var packet = new Packet(PacketId.TestPacket);
            packet.Write(testMessage);
            this.SendTcpData(packet);
        }

        #endregion
    }
}