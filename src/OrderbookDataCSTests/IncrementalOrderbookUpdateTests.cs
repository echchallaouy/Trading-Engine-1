using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TradingEngineServer.Instrument;
using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;
using TradingEngineServer.Price;

namespace OrderbookDataCSTests
{
    [TestClass]
    public class IncrementalOrderbookUpdateTests
    {
        [TestMethod]
        public void OrderbookLevel_UpdateType_Delete()
        {
            // Empty limit (empty price level with no orderbook entries)
            var limit = new Limit(0);
            var incrementalOrderbookUpdate = OrderbookUtilities.CreateIncrementalOrderbookUpdate(limit, DateTime.UtcNow);

            Assert.AreEqual(IncrementalOrderbookUpdateType.Delete, incrementalOrderbookUpdate.UpdateType);
            Assert.AreEqual(OrderbookEntryType.Null, incrementalOrderbookUpdate.EntryType);
            Assert.AreEqual(SecurityConstants.InvalidSecurityId, incrementalOrderbookUpdate.SecurityId);
            Assert.AreEqual(0u, incrementalOrderbookUpdate.OrderCount);
            Assert.AreEqual(PriceConstants.InvalidPrice, incrementalOrderbookUpdate.Price);
            Assert.AreEqual(0u, incrementalOrderbookUpdate.Quantity);
            Assert.AreEqual(0, incrementalOrderbookUpdate.IncrementalOrderbookUpdateEntries.Count);
        }

        [TestMethod]
        public void OrderbookLevel_UpdateType_New()
        {
            const long price = 10;
            const int securityId = 1;
            const bool isBuySide = true;
            Limit limit = CreateLimitWithOneEntry(price, securityId, isBuySide);

            var incrementalOrderbookUpdate = OrderbookUtilities.CreateIncrementalOrderbookUpdate(limit, DateTime.UtcNow);

            Assert.AreEqual(IncrementalOrderbookUpdateType.New, incrementalOrderbookUpdate.UpdateType);
            Assert.AreEqual(OrderbookEntryType.Bid, incrementalOrderbookUpdate.EntryType);
            Assert.AreEqual(securityId, incrementalOrderbookUpdate.SecurityId);
            Assert.AreEqual(1u, incrementalOrderbookUpdate.OrderCount);
            Assert.AreEqual(price, incrementalOrderbookUpdate.Price);
            Assert.AreEqual(10u, incrementalOrderbookUpdate.Quantity);
            Assert.AreEqual(1, incrementalOrderbookUpdate.IncrementalOrderbookUpdateEntries.Count);
        }

        [TestMethod]
        public void OrderbookLevel_UpdateType_Change()
        {
            const long price = 10;
            const int securityId = 1;
            const bool isBuySide = true;
            Limit limit = CreateLimitWithTwoEntries(price, securityId, isBuySide);

            var incrementalOrderbookUpdate = OrderbookUtilities.CreateIncrementalOrderbookUpdate(limit, DateTime.UtcNow);

            Assert.AreEqual(IncrementalOrderbookUpdateType.Change, incrementalOrderbookUpdate.UpdateType);
            Assert.AreEqual(OrderbookEntryType.Bid, incrementalOrderbookUpdate.EntryType);
            Assert.AreEqual(securityId, incrementalOrderbookUpdate.SecurityId);
            Assert.AreEqual(2u, incrementalOrderbookUpdate.OrderCount);
            Assert.AreEqual(price, incrementalOrderbookUpdate.Price);
            Assert.AreEqual(15u, incrementalOrderbookUpdate.Quantity);
            Assert.AreEqual(2, incrementalOrderbookUpdate.IncrementalOrderbookUpdateEntries.Count);
        }

        private static Limit CreateLimitWithOneEntry(long price, int securityId, bool isBuySide)
        {
            var limit = new Limit(price);
            var orderbookEntry = new OrderbookEntry(new Order(new OrderCore(0, string.Empty, securityId), price, 10, isBuySide), limit);
            limit.Head = orderbookEntry;
            limit.Tail = orderbookEntry;
            return limit;
        }

        private static Limit CreateLimitWithTwoEntries(long price, int securityId, bool isBuySide)
        {
            var limit = new Limit(price);
            var orderbookEntry = new OrderbookEntry(new Order(new OrderCore(0, string.Empty, securityId), price, 10, isBuySide), limit);
            var orderbookEntryTail = new OrderbookEntry(new Order(new OrderCore(0, string.Empty, securityId), price, 5, isBuySide), limit);
            orderbookEntry.Next = orderbookEntryTail;
            limit.Head = orderbookEntry;
            orderbookEntryTail.Previous = limit.Head;
            limit.Tail = orderbookEntryTail;
            return limit;
        }
    }
}
