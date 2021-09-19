using Microsoft.VisualStudio.TestTools.UnitTesting;

using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace OrderbookCSTest
{
    [TestClass]
    public class FifoOrderbookTests
    {
        private static readonly IOrderCore _testOrderBase = new OrderCore(0, "Test Username", 0);
        private static readonly Security _fifoSecurity = new Security()
        {
            AllocationAlgorithm = AllocationAlgorithm.Fifo,
        };

        [TestMethod]
        public void FifoOrderbook_MatchTwoOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(_fifoSecurity);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 10, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);

            var matchResult = fifoOrderbook.Match();

            Assert.AreEqual(2, matchResult.Fills.Count);
            Assert.AreEqual(1, matchResult.Trades.Count);
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(askOrderOrderId));
        }

        [TestMethod]
        public void FifoOrderbook_MatchTwoOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(_fifoSecurity);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 10, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);

            var matchResult = fifoOrderbook.Match();

            Assert.AreEqual(2, matchResult.Fills.Count);
            Assert.AreEqual(1, matchResult.Trades.Count);
            Assert.AreEqual(true, fifoOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(askOrderOrderId));
        }

        [TestMethod]
        public void FifoOrderbook_MatchThreeOrders_BidRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(_fifoSecurity);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), 10_000, 20, false); // Sell side hits 2 bids
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            // Asks wiped out.
            var matchResult = fifoOrderbook.Match();

            Assert.AreEqual(4, matchResult.Fills.Count);
            Assert.AreEqual(2, matchResult.Trades.Count);
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(true, fifoOrderbook.ContainsOrder(secondbuyOrderOrderId));
        }

        [TestMethod]
        public void FifoOrderbook_MatchThreeOrders_AskRemainsResting()
        {
            const long askOrderOrderId = 0;
            const long buyOrderOrderId = 1;
            const long secondbuyOrderOrderId = 2;
            const long askPrice = 10_000;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(_fifoSecurity);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 100, false); // Masive ask wall.
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), 10_001, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), 10_001, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            var matchResult = fifoOrderbook.Match();
            var spread = fifoOrderbook.GetSpread();

            Assert.AreEqual(4, matchResult.Fills.Count);
            Assert.AreEqual(2, matchResult.Trades.Count);
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(true, fifoOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(askPrice, spread.Ask.Value);
            Assert.AreEqual(false, spread.Bid.HasValue);
        }

        [TestMethod]
        public void FifoOrderbook_MatchFourOrders_PerfectMatch()
        {
            const long askOrderOrderId = 0;
            const long secondAskOrderOrderId = 1;
            const long buyOrderOrderId = 2;
            const long secondbuyOrderOrderId = 3;
            const long askPrice = 10_000;
            const long bidPrice = 10_001;
            MatchingOrderbook fifoOrderbook = OrderbookFactory.CreateOrderbook(_fifoSecurity);
            var askOrder = new Order(new OrderCore(askOrderOrderId, string.Empty, 0), askPrice, 10, false);
            var askOrder2 = new Order(new OrderCore(secondAskOrderOrderId, string.Empty, 0), askPrice, 20, false);
            var buyOrder = new Order(new OrderCore(buyOrderOrderId, string.Empty, 0), bidPrice, 15, true);
            var buyOrder2 = new Order(new OrderCore(secondbuyOrderOrderId, string.Empty, 0), bidPrice, 15, true);

            fifoOrderbook.AddOrder(askOrder);
            fifoOrderbook.AddOrder(askOrder2);
            fifoOrderbook.AddOrder(buyOrder);
            fifoOrderbook.AddOrder(buyOrder2);

            var matchResult = fifoOrderbook.Match();
            var spread = fifoOrderbook.GetSpread();

            Assert.AreEqual(6, matchResult.Fills.Count);
            Assert.AreEqual(3, matchResult.Trades.Count);
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(buyOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(askOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(secondbuyOrderOrderId));
            Assert.AreEqual(false, fifoOrderbook.ContainsOrder(secondAskOrderOrderId));
            Assert.AreEqual(false, spread.Ask.HasValue);
            Assert.AreEqual(false, spread.Bid.HasValue);
        }
    }
}
