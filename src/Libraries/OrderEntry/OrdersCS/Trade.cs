using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Trades
{
    public record Trade(DateTime EventTime, int SecurityId, long Price, uint Quantity, string ExecutionId, List<TradeOrderIdEntry> TradeOrderIdEntries);
}
