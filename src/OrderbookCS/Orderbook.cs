using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.Orderbook
{

    public class Orderbook : IRetrievalOrderbook
    {
        // PRIVATE FIELDS // 
        private readonly Security _inst;
        private readonly IDictionary<long, OrderbookEntry> _orders = new Dictionary<long, OrderbookEntry>();
        private readonly SortedSet<Limit> _bidLimits = new SortedSet<Limit>(BidLimitComparer.Comparer);
        private readonly SortedSet<Limit> _askLimits = new SortedSet<Limit>(AskLimitComparer.Comparer);

        // CONSTRUCTOR // 

        public Orderbook(Security instrument)
        {
            _inst = instrument;
        }

        // PUBLIC ITNERFACE METHODS //

        public void AddOrder(Order order)
        {
            var baseLimit = new Limit() { Price = order.Price, };
            AddOrder(order, baseLimit, order.IsBuySide ? _bidLimits : _askLimits, _orders);
        }

        public void ChangeOrder(ModifyOrder modifyOrder)
        {
            // Push them to the back of the queue regardless of what they want to change.
            if (_orders.TryGetValue(modifyOrder.OrderId, out var obe))
            {
                RemoveOrder(modifyOrder.ToCancelOrder(), obe, _orders);
                AddOrder(modifyOrder.ToNewOrder(), obe.ParentLimit, modifyOrder.IsBuySide ? _bidLimits : _askLimits, _orders);
            }
        }

        public void RemoveOrder(CancelOrder cancelOrder)
        {
            if (_orders.TryGetValue(cancelOrder.OrderId, out OrderbookEntry obe))
            {
                RemoveOrder(cancelOrder, obe, _orders);
            }
        }

        public bool ContainsOrder(long orderId)
        {
            return _orders.ContainsKey(orderId);
        }

        public bool TryGetOrder(long orderId, out Order order)
        {
            if (_orders.TryGetValue(orderId, out var entry))
            {
                // Create a copy as to not allow the client to alter the state of the order.
                order = entry.Current.Clone();
                return true;
            }
            order = null;
            return false;
        }

        public ModifyOrderType GetModifyOrderType(ModifyOrder modifyOrder)
        {
            if (_orders.TryGetValue(modifyOrder.OrderId, out var entry))
            {
                if (entry.Current.Price != modifyOrder.Price && entry.Current.InitialQuantity != modifyOrder.Quantity)
                {
                    return ModifyOrderType.PriceAndQuantity;
                }
                else if (entry.Current.Price != modifyOrder.Price)
                {
                    return ModifyOrderType.Price;
                }
                else if (entry.Current.InitialQuantity != modifyOrder.Quantity)
                {
                    return ModifyOrderType.Quantity;
                }
                else
                {
                    return ModifyOrderType.NoChange;
                }
            }
            return ModifyOrderType.Unknown;
        }

        public int Count
        {
            get
            {
                return _orders.Count;
            }
        }

        public List<OrderbookEntry> GetAskOrders()
        {
            List<OrderbookEntry> askOrders = new List<OrderbookEntry>(_askLimits.Count);
            foreach (var askOrder in _askLimits)
            {
                OrderbookEntry listTraverser = askOrder.Head;
                while (listTraverser != null)
                {
                    askOrders.Add(listTraverser);
                    listTraverser = listTraverser.Next;
                }
            }
            return askOrders;
        }

        public List<OrderbookEntry> GetBuyOrders()
        {
            List<OrderbookEntry> buyOrders = new List<OrderbookEntry>(_bidLimits.Count);
            foreach (var bidOrder in _bidLimits)
            {
                OrderbookEntry listTraverser = bidOrder.Head;
                while (listTraverser != null)
                {
                    buyOrders.Add(listTraverser);
                    listTraverser = listTraverser.Next;
                }
            }
            return buyOrders;
        }

        public OrderbookSpread GetSpread()
        {
            long? bestAsk = null, bestBid = null;
            if (_askLimits.Any() && _askLimits.Min.Head != null)
                bestAsk = _askLimits.Min.Price;
            if (_bidLimits.Any() && _bidLimits.Max.Head != null)
                bestBid = _bidLimits.Max.Price;
            return new OrderbookSpread(bestBid, bestAsk);
        }

        // PRIVATE STATIC METHODS // 

        private static void AddOrder(Order order, Limit baseLimit, SortedSet<Limit> limitLevels, IDictionary<long, OrderbookEntry> internalBook)
        {
            OrderbookEntry newEntry;
            if (limitLevels.TryGetValue(baseLimit, out Limit limit))
            {
                // Append to the end of the list. 
                newEntry = new OrderbookEntry(order, limit);
                if (limit.Head == null)
                {
                    // Can't have a tail without a head.
                    limit.Head = newEntry;
                    limit.Tail = newEntry;
                }
                else
                {
                    OrderbookEntry tailProxy = limit.Tail;
                    newEntry.Previous = tailProxy;
                    tailProxy.Next = newEntry;
                    limit.Tail = newEntry;
                }
            }
            else
            {
                newEntry = new OrderbookEntry(order, baseLimit);
                limitLevels.Add(baseLimit);
                baseLimit.Head = newEntry;
                baseLimit.Tail = newEntry;
            }
            internalBook.Add(order.OrderId, newEntry);
        }

        private static void RemoveOrder(CancelOrder cancelOrder, OrderbookEntry obe, IDictionary<long, OrderbookEntry> internalBook)
        {
            RemoveOrder(cancelOrder.OrderId, obe, internalBook);
        }

        private static void RemoveOrder(long orderId, OrderbookEntry obe, IDictionary<long, OrderbookEntry> internalBook)
        {
            // 1. Deal with location of OrderbookEntry within the linked list.
            if (obe.Previous != null && obe.Next != null)
            {
                // We are in the middle of the list
                obe.Next.Previous = obe.Previous;
                obe.Previous.Next = obe.Next;
            }
            else if (obe.Previous != null)
            {
                // We are on the Tail
                obe.Previous.Next = null;
            }
            else if (obe.Next != null)
            {
                // We are on the Head.
                obe.Next.Previous = null;
            }
                

            // 2. Deal with OrderbookEntry on the Limit-level
            if (obe.ParentLimit.Head == obe && obe.ParentLimit.Tail == obe)
            {
                obe.ParentLimit.Head = null;
                obe.ParentLimit.Tail = null;
            }
            else if (obe.ParentLimit.Head == obe)
                obe.ParentLimit.Head = obe.Next;
            else if (obe.ParentLimit.Tail == obe)
                obe.ParentLimit.Tail = obe.Previous;

            internalBook.Remove(orderId);
        }
    }
}
