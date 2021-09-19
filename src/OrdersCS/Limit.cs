using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public class Limit
    {
        public Limit(long price)
        {
            Price = price;
        }
        public long Price { get; init; }
        public OrderbookEntry Head { get; set; }
        public OrderbookEntry Tail { get; set; }
        public uint GetLevelOrderCount()
        {
            uint orderCount = 0;
            OrderbookEntry headPointer = Head;
            while (headPointer != null)
            {
                if (headPointer.Current.CurrentQuantity != 0)
                    orderCount++;
                headPointer = headPointer.Next;
            }
            return orderCount;
        }
        public uint GetLevelOrderQuantity()
        {
            uint orderQuantity = 0;
            OrderbookEntry headPointer = Head;
            while (headPointer != null)
            {
                orderQuantity += headPointer.Current.CurrentQuantity;
                headPointer = headPointer.Next;
            }
            return orderQuantity;
        }
        public List<OrderRecord> GetLevelOrderRecords()
        {
            List<OrderRecord> orderMetaData = new List<OrderRecord>();
            OrderbookEntry headPointer = Head;
            uint queuePosition = 0;
            while (headPointer != null)
            {
                var currentOrder = headPointer.Current;
                if (currentOrder.CurrentQuantity != 0)
                    orderMetaData.Add(new OrderRecord(currentOrder.OrderId, currentOrder.CurrentQuantity,
                        currentOrder.IsBuySide, currentOrder.Username, currentOrder.SecurityId, queuePosition++));
                headPointer = headPointer.Next;
            }
            return orderMetaData;
        }
        public bool IsEmpty
        {
            get
            {
                return Head == null && Tail == null;
            }
        }
        public Side Side
        {
            get
            {
                if (IsEmpty)
                    return Side.Unknown;
                else
                {
                    return Head.Current.IsBuySide ? Side.Bid : Side.Ask;
                }
            }
        }
    }
}
