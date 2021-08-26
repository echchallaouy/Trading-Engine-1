using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingEngineServer.Orderbook.MatchingAlgorithm;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public class MatchingOrderbook : IMatchingOrderbook
    {
        private readonly IMatchingAlgorithm _matchingAlgorithm;
        private readonly IRetrievalOrderbook _orderbook;
        private readonly object _lock = new object();

        public MatchingOrderbook(IRetrievalOrderbook orderbook, IMatchingAlgorithm matchingAlgorithm)
        {
            _orderbook = orderbook;
            _matchingAlgorithm = matchingAlgorithm;
        }

        public int Count
        {
            get
            {
                lock (_lock)
                    return _orderbook.Count;
            }
        }

        public void AddOrder(Order order)
        {
            lock (_lock)
                _orderbook.AddOrder(order);
        }

        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            lock (_lock)
                _orderbook.ChangeOrder(modifyOrder);
        }

        public void RemoveOrder(CancelOrder cancelOrder)
        {
            lock (_lock)
                _orderbook.RemoveOrder(cancelOrder);
        }

        public bool TryGetOrder(long orderId, out Order order)
        {
            return _orderbook.TryGetOrder(orderId, out order);
        }

        public bool ContainsOrder(long orderId)
        {
            lock (_lock)
                return _orderbook.ContainsOrder(orderId);
        }

        public OrderbookSpread GetSpread()
        {
            lock (_lock)
                return _orderbook.GetSpread();
        }

        public ModifyOrderType GetModifyOrderType(ModifyOrder modifyOrder)
        {
            lock (_lock)
                return _orderbook.GetModifyOrderType(modifyOrder);
        }

        public MatchResult Match()
        {
            lock (_lock)
            {
                var bids = _orderbook.GetBuyOrders();
                var asks = _orderbook.GetAskOrders();
                var matchResult = _matchingAlgorithm.Match(bids, asks);

                // Remove all fully filled orders from the book.
                var fullyFilledOrders = matchResult.Fills.Where(f => f.IsCompleteFill);
                foreach (var fullyFilledOrder in fullyFilledOrders)
                    _orderbook.RemoveOrder(new CancelOrder(fullyFilledOrder.OrderBase));

                return matchResult;
            }
        }


    }
}
