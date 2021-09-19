using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Common;

namespace TradingEngineServer.Orders
{
    public class Order : IOrderCore
    {
        public Order(IOrderCore orderBase, long price, uint quantity, bool isBuySide)
        {
            // PUBLIC // 
            Price = price;
            IsBuySide = isBuySide;
            InitialQuantity = quantity;
            CurrentQuantity = quantity;
            
            // PRIVATE // 
            _orderBase = orderBase;
        }

        public Order(ModifyOrder modifyOrder) : this(modifyOrder, 
            modifyOrder.Price, 
            modifyOrder.Quantity, 
            modifyOrder.IsBuySide)
        { }

        // PROPERTIES // 

        public long Price { get; private set; }
        public bool IsBuySide { get; private set; }
        public uint InitialQuantity { get; private set; }
        public uint CurrentQuantity { get; private set; }
        public long OrderId => _orderBase.OrderId;
        public string Username => _orderBase.Username;
        public int SecurityId => _orderBase.SecurityId;

        public void IncreaseQuantity(uint quantityDelta)
        {
            CurrentQuantity += quantityDelta;
        }

        public void DecreaseQuantity(uint quantityDelta)
        {
            if (quantityDelta > CurrentQuantity)
                throw new InvalidOperationException($"Quantity Delta > Current Quantity (OrderId: {OrderId})");
            CurrentQuantity -= quantityDelta;
        }

        // TOSTRING //
        public override string ToString()
        {
            return $"[{_orderBase}] Price: {Price}, " +
                $"InitialQuantity: {InitialQuantity}, CurrentQuantity: {CurrentQuantity}, " +
                $"IsBuySide: {IsBuySide}";
        }

        // PRIVATE //

        private readonly IOrderCore _orderBase;
    }
}
