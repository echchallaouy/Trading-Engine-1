using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TradingEngineServer.Orders
{
    internal sealed class OrderIdGenerator
    {
        private static long _orderId;

        static OrderIdGenerator()
        {
            // Seed _orderId with random number.
            _orderId = DateTime.Now.Ticks;
        }

        public static long GenerateOrderId()
        {
            return Interlocked.Increment(ref _orderId);
        }
    }
}
