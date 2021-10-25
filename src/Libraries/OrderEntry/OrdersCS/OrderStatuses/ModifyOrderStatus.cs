using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders.OrderStatuses
{
    public class ModifyOrderStatus : IModifyOrderStatus
    {
        public ModifyOrderStatus(ModifyOrder modifyOrder)
        {
            // PROPERTIES //
            CreationTime = DateTime.UtcNow;

            // FIELDS //
            _modifyOrder = modifyOrder;
        }

        // PROPERTIES //

        public DateTime CreationTime { get; private set; }
        public long OrderId => _modifyOrder.OrderId;
        public string Username => _modifyOrder.Username;
        public int SecurityId => _modifyOrder.SecurityId;
        public long Price => _modifyOrder.Price;
        public uint Quantity => _modifyOrder.Quantity;
        public bool IsBuySide => _modifyOrder.IsBuySide;

        // FIELDS //

        private readonly ModifyOrder _modifyOrder;
    }
}
