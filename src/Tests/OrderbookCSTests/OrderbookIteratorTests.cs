using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;
using TradingEngineServer.Orderbook.MatchingAlgorithm.OrderbookIterator;

namespace OrderbookCSTest
{
    [TestClass]
    public class OrderbookIteratorTests
    {
        private static readonly IEnumerable<OrderbookEntry> _obes = new List<OrderbookEntry>()
        {
            new OrderbookEntry(new Order(new OrderCore(0, string.Empty, 0), 1, 1, true), new Limit(0)),
            new OrderbookEntry(new Order(new OrderCore(1, string.Empty, 0), 1, 1, true), new Limit(0)),
        };

        [TestMethod]
        public void OrderbookIterator_IsDone()
        {
            var obIterator = new OrderbookEntryIterator(_obes);

            Assert.IsFalse(obIterator.IsDone());
            var firstItem = obIterator.CurrentItem();
            Assert.IsTrue(firstItem.Current.OrderId == 0);
            obIterator.Next();
            Assert.IsFalse(obIterator.IsDone());
            var secondItem = obIterator.CurrentItem();
            Assert.IsTrue(secondItem.Current.OrderId == 1);
            obIterator.Next();
            Assert.IsTrue(obIterator.IsDone());
            var thirdItem = obIterator.CurrentItemOrDefault();
            Assert.IsNull(thirdItem);
        }
    }
}
