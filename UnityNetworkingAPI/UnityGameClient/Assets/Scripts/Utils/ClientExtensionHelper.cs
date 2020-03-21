namespace UnityGameClient
{
    /// <summary>
    ///     Extensions that help to interact with the network client.
    /// </summary>
    public static class ServerExtensionHelper
    {
        /// <summary>
        ///     Sends tcp data throughout the network.
        /// </summary>
        public static void SendTcpData(this object obj, Packet packet)
        {
            NetworkClient.SendTcpData(packet);
        }

        /// <summary>
        ///     Sends udp data throughout the network.
        /// </summary>
        public static void SendUdpData(this object obj, Packet packet)
        {
            NetworkClient.SendUdpData(packet);
        }

        /// <summary>
        ///     Whether the network client is initialized.
        /// </summary>
        public static bool IsClientInitialized(this object obj)
        {
            return NetworkClient.IsInitialized;
        }

        /// <summary>
        ///     Gets the client id.
        /// </summary>
        public static int GetClientId(this object obj)
        {
            return NetworkClient.Id;
        }
    }
}