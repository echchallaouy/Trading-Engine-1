using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders.OrderStatuses
{
    public class NewOrderStatus : INewOrderStatus
    {
        public NewOrderStatus(Order order)
        {
            // PROPERTIES // 
            CreationTime = DateTime.UtcNow;

            // FIELDS //
            _order = order;
        }

        // PROPERTIES // 

        public DateTime CreationTime { get; private set; }
        public long OrderId => _order.OrderId;
        public string Username => _order.Username;
        public int SecurityId => _order.SecurityId;
        public long Price => _order.Price;
        public bool IsBuySide => _order.IsBuySide;
        public uint Quantity => _order.InitialQuantity;

        // FIELDS //

        private readonly Order _order;
    }
}
