using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderbookData
{
    public record TradeStatistics(int SecurityId, uint Volume, long Last, long High, long Low);
}
