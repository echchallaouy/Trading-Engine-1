using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orderbook.MatchingAlgorithm.OrderbookIterator
{
    public interface IIterator<T> where T : class
    {
        void First();
        void Next();
        bool IsDone();
        T CurrentItem();
    }

    public interface INullableIterator<T> : IIterator<T> where T : class
    {
        T CurrentItemOrDefault();
    }
}
