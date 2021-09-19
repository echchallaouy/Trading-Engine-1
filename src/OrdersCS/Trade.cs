using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Trades
{
    public class Trade
    {
        public DateTime EventTime { get; set; }
        public int SecurityId { get; set; }
        public long Price { get; set; }
        public uint Quantity { get; set; }
        public string ExecutionId { get; set; }
        public List<TradeOrderIdEntries> TradeOrderIdEntries { get; set; }
    }
}
