using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TradingEngineServer.Trades
{
    internal sealed class TradeIdGenerator
    {
        private static long _tradeNumber = 0;

        public static long GenerateTradeId()
        {
            return Interlocked.Increment(ref _tradeNumber);
        }
    }
}
