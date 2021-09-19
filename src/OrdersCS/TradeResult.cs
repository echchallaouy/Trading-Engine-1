using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Trades
{
    public class TradeResult
    {
        public TradeResult(Trade trade, Fill buyFill, Fill sellFill)
        {
            Trade = trade;
            BuyFill = buyFill;
            SellFill = sellFill;
        }

        public Trade Trade { get; private set; }
        public Fill BuyFill { get; private set; }
        public Fill SellFill { get; private set; }
    }
}
