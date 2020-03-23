using System.Collections.Generic;
using System.Linq;

namespace UnityGameServer
{
    public partial class NetworkServer
    {
        /// <summary>
        ///     Registry for clients by id.
        /// </summary>
        class ClientRegistry
        {
            /// <summary>
            ///     Register with all clients by identification.
            /// </summary>
            readonly Dictionary<int, Client> _clients;

            /// <summary>
            ///     Creates the registry.
            /// </summary>
            internal ClientRegistry()
            {
                _clients = new Dictionary<int, Client>();
            }

            /// <summary>
            ///     Amount of clients connected to the server.
            /// </summary>
            internal int ClientCount => _clients.Count;

            /// <summary>
            ///     Finds a client searching for the specific id. Returns null if the id is not present.
            /// </summary>
            internal Client GetClient(int id)
            {
                foreach (var key in _clients)
                    if (key.Value.Id == id)
                        return key.Value;
                return null;
            }

            /// <summary>
            ///     Returns an array with all clients connected to the server.
            /// </summary>
            internal Client[] GetClients()
            {
                return _clients.Values.ToArray();
            }

            /// <summary>
            ///     Registers a new client. Returns false in case the client id is already present in the registry.
            /// </summary>
            internal bool RegisterClient(Client client)
            {
                var id = client.Id;
                if (HasClient(id))
                    return false;

                _clients.Add(id, client);
                return true;
            }

            /// <summary>
            ///     Unregisters a client from the registry.
            /// </summary>
            internal void UnregisterClient(Client client)
            {
                _clients.Remove(client.Id);
            }

            /// <summary>
            ///     Checks if an id is already present in the registry.
            /// </summary>
            internal bool HasClient(int id)
            {
                return _clients.ContainsKey(id);
            }
        }
    }
}