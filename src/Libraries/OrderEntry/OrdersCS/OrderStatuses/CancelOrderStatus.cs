using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders.OrderStatuses
{
    public class CancelOrderStatus : ICancelOrderStatus
    {
        public CancelOrderStatus(CancelOrder cancelOrder)
        {
            // PROPERTIES //
            CreationTime = DateTime.UtcNow;

            // FIELDS //
            _cancelOrder = cancelOrder;
        }

        // PROPERTIES //

        public DateTime CreationTime { get; private set; }
        public long OrderId => _cancelOrder.OrderId;
        public string Username => _cancelOrder.Username;
        public int SecurityId => _cancelOrder.SecurityId;

        // FIELDS //

        private readonly CancelOrder _cancelOrder;
    }
}
