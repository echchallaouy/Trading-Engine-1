using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderEntryCommunicationServer
{
    public interface ICache<Key, Value>
    {
        void Add(Key key, Value value);
        bool Remove(Key key);
        List<Value> GetAll();
        Value Get(Key key);
        bool TryGet(Key key, out Value value);
        bool Contains(Key key); 
    }
}
