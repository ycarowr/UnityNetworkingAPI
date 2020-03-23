using System.Net;
using UnityEngine;

namespace UnityGameServer
{
    public partial class NetworkServer
    {
        public partial class Client
        {
            /// <summary>
            ///     Wrapper for the client UDP implementation in the server side.
            /// </summary>
            public class Udp
            {
                readonly ServerConfigs _configs;

                /// <summary>
                ///     Private Constructor.
                /// </summary>
                Udp()
                {
                }

                /// <summary>
                ///    Creates a UDP connection with the client.
                /// </summary>
                public Udp(int id, ServerConfigs configs)
                {
                    _configs = configs;
                    IpEndPoint = null;
                    Id = id;
                }

                public bool IsConnected => IpEndPoint != null;
                
                /// <summary>
                ///     Id of the client.
                /// </summary>
                public int Id { get; }
                
                /// <summary>
                ///     Ip end point of the client's connection.
                /// </summary>
                public IPEndPoint IpEndPoint { get; private set; }

                /// <summary>
                ///     Connects to the client.
                /// </summary>
                public void ConnectUdp(IPEndPoint ipEndPoint)
                {
                    IpEndPoint = ipEndPoint;
                    Debug.Log("Connect Client UDP: "+ipEndPoint);
                }

                /// <summary>
                ///     Disconnects the client.
                /// </summary>
                public void Disconnect()
                {
                    IpEndPoint = null;
                }

                /// <summary>
                ///     Handles the data.
                /// </summary>
                public void HandleData(Packet packetData)
                {
                    var packet = new Packet(packetData.ToArray());
                    var packetLength = packet.ReadInt();
                    var clientId = packet.ReadInt();
                    var client = _clientRegistry.GetClient(clientId);
                    var packetId = (PacketId) packet.ReadInt();
                    packet.SetId(packetId);
                    ThreadManager.Schedule(() => client.HandlePacket(packetId, packet));
                }
            }
        }
    }
}