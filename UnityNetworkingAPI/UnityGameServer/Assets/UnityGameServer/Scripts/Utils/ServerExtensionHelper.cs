namespace UnityGameServer
{
    public static class ServerExtensionHelper
    {
        /// <summary>
        ///     Sends tcp data to target client. 
        /// </summary>
        public static void SendTcpData(this object obj, int clientId, Packet packet)
        {
            NetworkServer.SendTcpData(clientId, packet);
        }

        /// <summary>
        ///     Sends udp data to target client. 
        /// </summary>
        public static void SendUdpData(this object obj, int clientId, Packet packet)
        {
            NetworkServer.SendUdpData(clientId, packet);
        }

        /// <summary>
        ///     Sends tcp data to all clients. 
        /// </summary>
        public static void SendTcpDataAll(this object obj, Packet packet)
        {
            NetworkServer.SendTcpDataToAll(packet);
        }

        /// <summary>
        ///    Sends udp data to all clients. 
        /// </summary>
        public static void SendUdpDataAll(this object obj, Packet packet)
        {
            NetworkServer.SendUdpDataToAll(packet);
        }

        /// <summary>
        ///    Gets a connected client by the id.
        /// </summary>
        public static NetworkServer.IClient GetClient(this object obj, int id)
        {
            return NetworkServer.GetClient(id);
        }

        /// <summary>
        ///     Gets all the connected clients.
        /// </summary>
        public static NetworkServer.IClient[] GetClients(this object obj)
        {
            return NetworkServer.GetClients();
        }

        /// <summary>
        ///     Whether the server is initialized or not.
        /// </summary>
        public static bool IsServerInitialized(this object obj)
        {
            return NetworkServer.IsInitialized;
        }

        /// <summary>
        ///     Whether the server is able to be found by the clients or not.
        /// </summary>
        public static bool IsServerConnected(this object obj)
        {
            return NetworkServer.IsConnected;
        }
    }
}