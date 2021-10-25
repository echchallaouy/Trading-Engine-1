using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;
using TradingEngineServer.Trades;

namespace TradingEngineServer.MarketData
{
    /// <summary>
    /// Responsible for sending updates from EDEN to EXODUS.
    /// </summary>
    public class MarketDataPublisher : IMarketDataPublisher
    {
        public MarketDataPublisher(IMarketDataClient marketDataClient)
        {

        }

        public Task PublishIncrementalOrderbookUpdateAsync(IncrementalOrderbookUpdate incrementalOrderbookUpdates, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task PublishIncrementalOrderbookUpdateAsync(OrderbookSlice obs, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task PublishIncrementalOrderbookUpdatesAsync(List<IncrementalOrderbookUpdate> incrementalOrderbookUpdates, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task PublishTradeAsync(Trade trades, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task PublishTradesAsync(List<Trade> trades, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
