using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.OrderbookData;

namespace TradingEngineServer.MarketData
{
    interface IMarketDataPublisher
    {
        Task PublishTradesAsync(List<Trades.Trade> trades, CancellationToken token);
        Task PublishIncrementalOrderbookUpdatesAsync(List<IncrementalOrderbookUpdate> incrementalOrderbookUpdates, CancellationToken token);
    }
}
