using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderEntryCommunication
{
    public class ServerClientStore : ICache<string, OrderEntryServerClient>
    {
        public ServerClientStore()
        { }

        public void Add(string key, OrderEntryServerClient value)
        {
            lock (_lock)
                _clients.Add(key, value);
        }

        public bool Contains(string key)
        {
            lock (_lock)
                return _clients.ContainsKey(key);
        }

        public OrderEntryServerClient Get(string key)
        {
            lock (_lock)
                return _clients[key];
        }

        public bool Remove(string key)
        {
            lock (_lock)
            {
                if (_clients.TryGetValue(key, out var client))
                {
                    _clients.Remove(key);
                    return true;
                }
                else return false;
            }
        }

        public bool TryGet(string key, out OrderEntryServerClient value)
        {
            lock (_lock)
                return _clients.TryGetValue(key, out value);
        }

        public List<OrderEntryServerClient> GetAll()
        {
            lock (_lock)
                return new List<OrderEntryServerClient>(_clients.Values);
        }

        private readonly Dictionary<string, OrderEntryServerClient> _clients =
            new Dictionary<string, OrderEntryServerClient>();
        private readonly object _lock = new object();
    }
}
