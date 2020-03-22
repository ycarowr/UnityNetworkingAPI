using UnityEngine;
using UnityGameServer;

namespace Demo
{
    /// <summary>
    ///     Demo of a server application.
    /// </summary>
    public class DemoServerApplication : BaseServerApplication
    {
        protected override void OnInitialize()
        {
            Logger.Log("Demo Server Initialized!", Color.black, GetType().ToString());
        }

        protected override void OnClientConnect(NetworkServer.IClient client)
        {
            Debug.Log($"A client has connect with Id {client.Id}");
        }

        protected override void OnClientDisconnect(NetworkServer.IClient client)
        {
            Debug.Log($"A client has disconnect with Id {client.Id}");
        }

        protected override void OnClientReceivedPacket(NetworkServer.IClient client, Packet packet)
        {
            // Debug.Log($"The client {client.Id} has sent a message, id: {packet.Id}");
            if (packet.Id == PacketId.TestPacket)
            {
                Debug.Log($"The client {client.Id} has sent a test message, msg: {packet.ReadString()}");
            }
        }

        #region Test

        [Header("Demo Test"), SerializeField] string testMessage;

        [Button]
        void SendTestMessage()
        {
            var packet = new Packet(PacketId.TestPacket);
            packet.Write(testMessage);
            NetworkServer.SendTcpDataToAll(packet);
        }

        #endregion
    }
}