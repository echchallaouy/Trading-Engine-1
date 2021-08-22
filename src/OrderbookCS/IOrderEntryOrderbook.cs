﻿using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public interface IReadOnlyOrderbook
    {
        bool ContainsOrder(long orderId);
        bool TryGetOrder(long orderId, out Order order);
        OrderbookSpread GetSpread();
        int Count { get; }
    }
    public interface IOrderEntryOrderbook : IReadOnlyOrderbook
    {
        void AddOrder(Order order);
        void ChangeOrder(ModifyOrder modifyOrder);
        void RemoveOrder(CancelOrder cancelOrder);
    }

    public interface IRetrievalOrderbook : IOrderEntryOrderbook
    {
        List<OrderbookEntry> GetAskOrders();
        List<OrderbookEntry> GetBuyOrders();
    }
}
