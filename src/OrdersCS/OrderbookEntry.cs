using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orders
{
    public class OrderbookEntry
    {
        public OrderbookEntry(Order currentOrder, Limit parentLimit)
        {
            Current = currentOrder;
            ParentLimit = parentLimit;
            CreationTime = DateTime.UtcNow;
        }
        public DateTime CreationTime { get; private set; }
        public Order Current { get; private set; }
        public Limit ParentLimit { get; private set; }
        public OrderbookEntry Next { get; set; } = null;
        public OrderbookEntry Previous { get; set; } = null;

        public override string ToString()
        {
            return $"{Current}";
        }
    }
}
