using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace OrderbookCSTest
{
    [TestClass]
    public class ProRataOrderbookTests
    {
        private static readonly Security _security = new Security()
        {
            AllocationAlgorithm = AllocationAlgorithm.ProRata,
        };

        [TestMethod]
        public void ProRataOrderbook_MatchTwoOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 10, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);

            var matchResult = prorataOrderbook.Match();

            Assert.AreEqual(2, matchResult.Fills.Count);
            Assert.AreEqual(1, matchResult.Trades.Count);
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(askOrderOrderId));
        }

        [TestMethod]
        public void ProrataOrderbook_MatchTwoOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);

            var matchResult = prorataOrderbook.Match();

            Assert.AreEqual(2, matchResult.Fills.Count);
            Assert.AreEqual(1, matchResult.Trades.Count);
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(askOrderOrderId));
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            // Later buy order matched first because it is on the same price level as the previous and has a higher quantity
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 16, true); 

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            // Asks wiped out.
            var matchResult = prorataOrderbook.Match();

            Assert.AreEqual(4, matchResult.Fills.Count);
            Assert.AreEqual(2, matchResult.Trades.Count);
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondbuyOrderOrderId));
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_AskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 100, false); // Masive ask wall.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var matchResult = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(4, matchResult.Fills.Count);
            Assert.AreEqual(2, matchResult.Trades.Count);
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(askPrice, spread.Ask.Value);
            Assert.AreEqual(false, spread.Bid.HasValue);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchThreeOrders_SameQuantityTimeOrderPreserved()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 16, false);
            // Two buy orders have the same quantity, relative ordering is preserved and first buyOrder is matched first.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var matchResult = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(4, matchResult.Fills.Count);
            Assert.AreEqual(2, matchResult.Trades.Count);
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(bidPrice, spread.Bid.Value);
            Assert.AreEqual(false, spread.Ask.HasValue);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchFourOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(askOrder2);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);

            var matchResult = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(6, matchResult.Fills.Count);
            Assert.AreEqual(3, matchResult.Trades.Count);
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondAskOrderOrderId));
            Assert.AreEqual(false, spread.Ask.HasValue);
            Assert.AreEqual(false, spread.Bid.HasValue);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchFourOrders_FirstAskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long thirdBuyOrderId = 4; // 
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            // Second ask order has higher quantity that the total of the resting bids. It gets fully filled
            // and the first askOrder remains resting with a quantity of 1.
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 21, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder3 = new Order(new OrderCore(thirdBuyOrderId, string.Empty, 0), askPrice - 1, 1, true); // This order does not get matched.

            prorataOrderbook.AddOrder(askOrder);
            prorataOrderbook.AddOrder(askOrder2);
            prorataOrderbook.AddOrder(buyOrder);
            prorataOrderbook.AddOrder(buyOrder2);
            prorataOrderbook.AddOrder(buyOrder3);

            var matchResult = prorataOrderbook.Match();
            var spread = prorataOrderbook.GetSpread();

            Assert.AreEqual(6, matchResult.Fills.Count);
            Assert.AreEqual(3, matchResult.Trades.Count);
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(false, prorataOrderbook.ContainsOrder(secondAskOrderOrderId));
            Assert.AreEqual(true, prorataOrderbook.ContainsOrder(thirdBuyOrderId));
            Assert.AreEqual(true, spread.Ask.HasValue);
            Assert.AreEqual(true, spread.Bid.HasValue);
            Assert.AreEqual(1, spread.Spread.Value);
        }

        [TestMethod]
        public void ProRataOrderbook_MatchNoOrders()
        {
            MatchingOrderbook prorataOrderbook = OrderbookFactory.CreateOrderbook(_security);
            Assert.ThrowsException<MatchException>(() => prorataOrderbook.Match());
        }
    }
}
