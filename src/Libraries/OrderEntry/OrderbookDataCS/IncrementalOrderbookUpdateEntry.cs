using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderbookData
{
    public record IncrementalOrderbookUpdateEntry(long OrderId, uint Quantity, uint TheoreticalQueuePosition);
}
