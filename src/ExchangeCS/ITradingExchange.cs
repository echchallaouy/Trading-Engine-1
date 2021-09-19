using System;
using System.Collections.Generic;
using System.Text;

using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;

namespace TradingEngineServer.Exchange
{
    public interface IOrderbookRetriever
    {
        bool TryGetOrderbook(int securityId, out IMatchingOrderbook orderbook);
    }

    public interface ITradingExchange : IOrderbookRetriever
    {
        int GetExchangeId();
        string GetExchangeName();
    }
}
