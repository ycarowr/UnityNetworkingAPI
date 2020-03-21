using System;

namespace UnityGameServer
{
    public static partial class NetworkServer
    {
        /// <summary>
        ///     A client in the server application side.
        /// </summary>
        public interface IClient
        {
            int Id { get; }
            event Action<Packet> OnReceivePacket;
        }
    }
}