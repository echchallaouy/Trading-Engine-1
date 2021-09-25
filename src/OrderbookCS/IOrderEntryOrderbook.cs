using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public interface IReadOnlyOrderbook
    {
        bool IsCrossed();
        bool ContainsOrder(long orderId);
        bool TryGetOrder(long orderId, out OrderRecord order);
        ModifyOrderType GetModifyOrderType(ModifyOrder modifyOrder);
        OrderbookSlice GetOrderbookSlice(long price);
        OrderbookSpread GetSpread();
        int Count { get; }
    }
    public interface IOrderEntryOrderbook : IReadOnlyOrderbook
    {
        void AddOrder(Order order);
        void ChangeOrder(ModifyOrder modifyOrder);
        void RemoveOrder(CancelOrder cancelOrder);
        void CancelAll(string username);
        void CancelAll(List<long> ids);
    }

    public interface IRetrievalOrderbook : IOrderEntryOrderbook
    {
        List<OrderbookEntry> GetAskOrders();
        List<OrderbookEntry> GetBuyOrders();
    }
}
