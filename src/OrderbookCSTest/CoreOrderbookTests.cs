using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;
using TradingEngineServer.Rejects;

namespace OrderbookCSTest
{
    [TestClass]
    public class CoreOrderbookTests
    {
        [TestMethod]
        public void Orderbook_AddSingleOrder_Success()
        {
            // 1
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(0, "Test", 1), 1_000, 10, true));

            // 2
            int actual = ob.Count;
            const int expected = 1;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_AddTwoOrders_Success()
        {
            // 1
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(0, "Test", 1), 1_000, 10, true));
            ob.AddOrder(new Order(new OrderCore(1, "Test", 1), 1_000, 10, false));

            // 2
            int actual = ob.Count;
            const int expected = 2;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_AddOrderThenRemoveOrder_Success()
        {
            // 1
            const long orderId = 0;
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(orderId, "Test", 1), 1_000, 10, true));
            ob.RemoveOrder(new CancelOrder(new OrderCore(orderId, "Test", 1)));

            // 2
            int actual = ob.Count;
            const int expected = 0;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_RemoveNonExistantOrder_Noop()
        {
            // Nothing happens when we try to remove an order that doesn't exist.
            // 1
            Orderbook ob = new Orderbook(default);
            ob.RemoveOrder(new CancelOrder(new OrderCore(0, "Test", 1)));

            // 2
            int actual = ob.Count;
            const int expected = 0;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_AddOrderThenModify_Success()
        {
            // 1
            const long orderId = 0;
            const uint modifyOrderQuantity = 5;
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(orderId, "Test", 1), 1_000, 10, true));
            ob.ChangeOrder(new ModifyOrder(new OrderCore(orderId, "Test", 1), 1_000, modifyOrderQuantity, true));

            // 2
            var buyOrders = ob.GetBuyOrders();

            int actual = ob.Count;
            const int expected = 1;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_AddTwoOrdersThenModify_Success()
        {
            // 1
            const long orderId = 0;
            const uint modifyOrderQuantity = 5;
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(orderId, "Test", 1), 1_000, 10, true));
            ob.AddOrder(new Order(new OrderCore(1, "Person", 1), 1_000, 10, true)); 
            ob.ChangeOrder(new ModifyOrder(new OrderCore(orderId, "Test", 1), 1_000, modifyOrderQuantity, true)); // Modified order should be moved to back

            // 2
            var buyOrders = ob.GetBuyOrders();

            int actual = ob.Count;
            const int expected = 2;
            var lastOrder = buyOrders.Last();

            // 3
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(orderId, lastOrder.Current.OrderId);
        }

        [TestMethod]
        public void Orderbook_ModifyOrderSwitchSides_Success()
        {
            // 1
            const long orderId = 0;
            const uint modifyOrderQuantity = 5;
            const bool IsBuySide = false;
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(orderId, "Test", 1), 1_000, 10, true));
            ob.ChangeOrder(new ModifyOrder(new OrderCore(orderId, "Test", 1), 1_000, modifyOrderQuantity, IsBuySide)); // Switches side

            // 2
            int actual = ob.Count;
            const int expected = 1;
            var askOrders = ob.GetAskOrders();
            var lastOrder = askOrders[^1];

            // 3
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(orderId, lastOrder.Current.OrderId);
            Assert.AreEqual(IsBuySide, lastOrder.Current.IsBuySide);
        }

        [TestMethod]
        public void Orderbook_ModifyNonExistantOrder_RequestRejects()
        {
            // 1
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(0, "Test", 1), 1_000, 10, true));
            ob.ChangeOrder(new ModifyOrder(new OrderCore(0, "Test", 1), 1_000, 5, false));

            // 2
            int actual = ob.Count;
            const int expected = 1;

            // 3
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Orderbook_BidOrders_AreDescending()
        {
            // 1
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(0, "Test", 1), 1_000, 10, true));
            ob.AddOrder(new Order(new OrderCore(1, "Test", 1), 999, 10, true));

            // 2
            var bidOrders = ob.GetBuyOrders();
            int actualSize = bidOrders.Count;
            const int expectedSize = 2;

            // 3
            Assert.AreEqual(expectedSize, actualSize);
            Assert.IsTrue(bidOrders[0].Current.Price > bidOrders[^1].Current.Price);
        }

        [TestMethod]
        public void Orderbook_AskOrders_AreAscending()
        {
            // 1
            Orderbook ob = new Orderbook(default);
            ob.AddOrder(new Order(new OrderCore(0, "Test", 1), 1_000, 10, false));
            ob.AddOrder(new Order(new OrderCore(1, "Test", 1), 1_001, 10, false));

            // 2
            var askOrders = ob.GetAskOrders();
            int actual = ob.Count;
            const int expected = 2;

            // 3
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(askOrders[0].Current.Price < askOrders[^1].Current.Price);
        }
    }
}
