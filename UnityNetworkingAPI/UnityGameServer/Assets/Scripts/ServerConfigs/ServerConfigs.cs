using UnityEngine;

namespace UnityGameServer
{
    [CreateAssetMenu]
    public class ServerConfigs : ScriptableObject
    {
        [Tooltip("Size of buffers when receiving network data."), SerializeField]
        int bufferSize = 4096;

        [Tooltip("Limit maximum of clients connected at the same time."), SerializeField]
        int limitOfConnections = 50;

        [Tooltip("The port where the TCP will be listening to."), SerializeField]
        int port = 26950;

        [Tooltip("The framerate of the Unity Server."), SerializeField]
        int serverFrameRate = 30;

        [Tooltip("The vSync of the Unity Server."), SerializeField]
        int serverVSyncDefault;

        public int BufferSize => bufferSize;
        public int LimitOfConnections => limitOfConnections;
        public int Port => port;
        public int ServerFrameRate => serverFrameRate;
        public int ServerVSyncDefault => serverVSyncDefault;
    }
}