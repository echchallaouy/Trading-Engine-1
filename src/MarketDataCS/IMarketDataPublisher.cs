using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;

namespace TradingEngineServer.MarketData
{
    interface IMarketDataPublisher
    {
        Task PublishTradeAsync(Trades.Trade trades, CancellationToken token);
        Task PublishTradesAsync(List<Trades.Trade> trades, CancellationToken token);
        Task PublishIncrementalOrderbookUpdateAsync(IncrementalOrderbookUpdate incrementalOrderbookUpdates, CancellationToken token);
        Task PublishIncrementalOrderbookUpdatesAsync(List<IncrementalOrderbookUpdate> incrementalOrderbookUpdates, CancellationToken token);
        Task PublishIncrementalOrderbookUpdateAsync(OrderbookSlice obs, CancellationToken token);
    }
}
