using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Common;

namespace TradingEngineServer.Orders
{
    public class ModifyOrder : IOrderCore
    {
        public ModifyOrder(OrderCore orderBase, long price, uint modifyQuantity, bool isBuySide)
        {
            // PUBLIC //
            Price = price;
            Quantity = modifyQuantity;
            IsBuySide = isBuySide;

            // PRIVATE //
            _orderBase = orderBase;
        }

        // PROPERTIES //

        public long Price { get; private set; }
        public uint Quantity { get; private set; }
        public bool IsBuySide { get; private set; }
        public long OrderId => _orderBase.OrderId;
        public string Username => _orderBase.Username;
        public int SecurityId => _orderBase.SecurityId;

        // METHODS //

        public CancelOrder ToCancelOrder()
        {
            return new CancelOrder(this);
        }
        public Order ToNewOrder()
        {
            return new Order(this);
        }

        // TOSTRING //
        public override string ToString()
        {
            return $"[{_orderBase}] Price: {Price}, Quantity: {Quantity}, IsBuySide: {IsBuySide}";
        }

        // FIELDS //

        private readonly IOrderCore _orderBase;
    }
}
