using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook.MatchingAlgorithm.OrderbookIterator
{
    public class OrderbookEntryIterator : INullableIterator<OrderbookEntry>
    {
        public OrderbookEntryIterator(IEnumerable<OrderbookEntry> orderbookEntries)
        {
            _orderbookEntries = orderbookEntries;
        }

        /// <summary>
        /// Positions the iterator at the first element.
        /// </summary>
        public void First()
        {
            _current = 0;
        }

        public void Next()
        {
            _current++;
        }

        public bool IsDone()
        {
            return _current == _orderbookEntries.Count();
        }

        public OrderbookEntry CurrentItem()
        {
            return _orderbookEntries.ElementAt(_current);
        }

        public OrderbookEntry CurrentItemOrDefault()
        {
            return _orderbookEntries.ElementAtOrDefault(_current);
        }

        private readonly IEnumerable<OrderbookEntry> _orderbookEntries = Enumerable.Empty<OrderbookEntry>();
        private int _current = 0;
    }
}
