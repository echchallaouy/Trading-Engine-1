using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook.MatchingAlgorithm.OrderbookIterator;
using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;
using TradingEngineServer.Trades;

namespace TradingEngineServer.Orderbook.MatchingAlgorithm
{
    public class LifoMatchingAlgorithm : IMatchingAlgorithm
    {
        public static IMatchingAlgorithm MatchingAlgorithm { get; } = new LifoMatchingAlgorithm();

        public MatchResult Match(IEnumerable<OrderbookEntry> bids, IEnumerable<OrderbookEntry> asks)
        {
            var eventTime = DateTime.UtcNow;
            var matchResult = new MatchResult();

            if (!bids.Any() || !asks.Any())
                return matchResult; // Can't match without both sides.

            // Cannot guarantee that bids are ordered in last-in-first-out order. Let's go ahead and do that.
            var reorderedBids = bids.GroupBy(x => x.ParentLimit.Price).Select(x => x.OrderByDescending(oe => oe.CreationTime))
                .SelectMany(x => x);
            var reorderedAsks = asks.GroupBy(x => x.ParentLimit.Price).Select(x => x.OrderByDescending(oe => oe.CreationTime))
                .SelectMany(x => x);

            var bidOrderIterator = new OrderbookEntryIterator(reorderedBids);
            var askOrderIterator = new OrderbookEntryIterator(reorderedAsks);

            OrderbookEntry orderToMatchBid = bidOrderIterator.CurrentItemOrDefault();
            OrderbookEntry orderToMatchAsk = askOrderIterator.CurrentItemOrDefault();

            do
            {
                if (orderToMatchAsk.Current.Price > orderToMatchBid.Current.Price)
                    break;  // No book match candidates.
                var remainingQuantityBid = orderToMatchBid.Current.CurrentQuantity;
                if (remainingQuantityBid == 0)
                {
                    bidOrderIterator.Next();
                    orderToMatchBid = bidOrderIterator.CurrentItemOrDefault();
                    continue;
                }
                var remainingQuantityAsk = orderToMatchAsk.Current.CurrentQuantity;
                if (remainingQuantityAsk == 0)
                {
                    askOrderIterator.Next();
                    orderToMatchAsk = askOrderIterator.CurrentItemOrDefault();
                    continue;
                }
                var fillQuantity = Math.Min(remainingQuantityAsk, remainingQuantityBid);

                orderToMatchBid.Current.DecreaseQuantity(fillQuantity);
                orderToMatchAsk.Current.DecreaseQuantity(fillQuantity);

                // TODO: This is duplicate code in all matching algorithms.
                // Think of refactoring this by including it elsewhere
                var tradeResult = TradeUtilities.CreateTradeAndFills(orderToMatchBid.Current, orderToMatchAsk.Current,
                    fillQuantity, AllocationAlgorithm.Lifo, eventTime);
                matchResult.AddTradeResult(tradeResult);
                bool buySideIsAggressor = orderToMatchBid.CreationTime > orderToMatchAsk.CreationTime;
                Limit relevantOrderbookLimit = buySideIsAggressor ? orderToMatchAsk.ParentLimit : orderToMatchBid.ParentLimit;
                var orderbookUpdate = OrderbookUtilities.CreateIncrementalOrderbookUpdate(relevantOrderbookLimit, eventTime);
                matchResult.AddIncrementalOrderbookUpdate(orderbookUpdate);

                // Lets move on. Or better said, let's move back!
                if (tradeResult.BuyFill.IsCompleteFill)
                {
                    bidOrderIterator.Next();
                    orderToMatchBid = bidOrderIterator.CurrentItemOrDefault();
                }
                if (tradeResult.SellFill.IsCompleteFill)
                {
                    askOrderIterator.Next();
                    orderToMatchAsk = askOrderIterator.CurrentItemOrDefault();
                }
            }
            while (orderToMatchBid != null && orderToMatchAsk != null);

            return matchResult;
        }
    }
}
