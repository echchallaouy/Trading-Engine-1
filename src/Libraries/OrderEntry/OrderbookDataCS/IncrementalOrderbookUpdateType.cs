using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderbookData
{
    public enum IncrementalOrderbookUpdateType
    {
        New,
        Change,
        Delete,
    }
}
