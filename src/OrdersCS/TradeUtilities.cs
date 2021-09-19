using System;
using System.Collections.Generic;
using System.Text;

using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Trades
{
    public sealed class TradeUtilities
    {
        public static TradeResult CreateTradeAndFills(Order bidOrder, Order askOrder, 
            uint fillQuantity, AllocationAlgorithm fillAlocAlgo, DateTime eventTime)
        {
            var tradeNumber = TradeIdGenerator.GenerateTradeId();
            var fillsIds = GetFillIds(tradeNumber);
            var executionId = GetTradeExecutionId(eventTime, tradeNumber);

            var buyFill = new Fill()
            {
                EventTime = eventTime,
                AllocationAlgorithm = fillAlocAlgo,
                FillQuantity = fillQuantity,
                IsCompleteFill = bidOrder.CurrentQuantity == 0,
                FillId = fillsIds.FirstFillId,
                OrderBase = new OrderCore(bidOrder.OrderId, bidOrder.Username, bidOrder.SecurityId),
                ExecutionId = executionId,
            };

            var askFill = new Fill()
            {
                EventTime = eventTime,
                AllocationAlgorithm = fillAlocAlgo,
                FillQuantity = fillQuantity,
                IsCompleteFill = askOrder.CurrentQuantity == 0,
                FillId = fillsIds.SecondFillId,
                OrderBase = new OrderCore(askOrder.OrderId, askOrder.Username, askOrder.SecurityId),
                ExecutionId = executionId,
            };

            var trade = new Trade()
            {
                EventTime = eventTime,
                SecurityId = bidOrder.SecurityId,
                Price = bidOrder.Price,
                Quantity = fillQuantity,
                ExecutionId = executionId,
                TradeOrderIdEntries = new List<TradeOrderIdEntries>()
                {
                    new TradeOrderIdEntries(bidOrder.OrderId, fillQuantity),
                    new TradeOrderIdEntries(askOrder.OrderId, fillQuantity),
                },
            };

            return new TradeResult(trade, buyFill, askFill);
        }

        private static (long FirstFillId, long SecondFillId) GetFillIds(long tradeNumber)
        {
            return (2 * tradeNumber, 2 * tradeNumber + 1);
        }

        private static string GetTradeExecutionId(DateTime tradeTime, long tradeNumber)
        {
            return $"{tradeTime:yyyyMMdd}:T:{tradeNumber:000000000000}";
        }
    }
}
