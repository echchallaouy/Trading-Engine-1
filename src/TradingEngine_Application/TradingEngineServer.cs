using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TradingEngineServer.Core.Configuration;
using TradingEngineServer.Logging;
using TradingEngineServer.Exchange;
using TradingEngineServer.Orders;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Rejects;

using TradingEngineServer.OrderEntryCommunicationServer;
using System.Linq;
using TradingEngineServer.Orders.OrderStatuses;
using TradingEngineServer.OrderbookData;

namespace TradingEngineServer.Core
{
    class TradingEngineServer : BackgroundService, ITradingEngine, ITradingUpdateProcessor
    {
        private readonly TradingEngineServerConfiguration _engineConfiguration;
        private readonly ITextLogger _textLogger;
        private readonly ITradingExchange _exchange;

        public TradingEngineServer(IOptions<TradingEngineServerConfiguration> engineConfiguration,
            ITradingExchange exchange,
            ITextLogger textLogger)
        {
            _engineConfiguration = engineConfiguration.Value ?? throw new ArgumentNullException(nameof(engineConfiguration));
            _exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
            _textLogger = textLogger ?? throw new ArgumentNullException(nameof(textLogger));

        }
        public Task<ExchangeResult> ProcessOrderAsync(Order order)
        {
            var now = DateTime.UtcNow;
            _textLogger.Information(nameof(TradingEngineServer), $"Handling NewOrder: {order}");
            if (_exchange.TryGetOrderbook(order.SecurityId, out var orderbook))
            {
                if (RejectGenerator.TryRejectNewOrder(order, orderbook, out var rejection))
                {
                    return Task.FromResult(new ExchangeResult(rejection));
                }
                else
                {
                    // THIS METHOD IS INCOMPLETE
                    orderbook.AddOrder(order);
                    if (orderbook.TryMatch(out MatchResult matchResults))
                    {
                        // Publish Market Data
                        // ...
                        //

                        // Publish fills.
                        return Task.FromResult(new ExchangeResult(matchResults.Fills));
                    }
                    else
                    {
                        // Create a single orderbook update outside of those in a match.
                        var orderRecords = orderbook.GetOrderbookSlice(order.Price).OrderRecords;
                        var orderLevel = new IncrementalOrderbookUpdate
                        {
                            SecurityId = order.SecurityId,
                            EntryType = order.IsBuySide ? OrderbookEntryType.Bid : OrderbookEntryType.Ask,
                            EventTime = now, // Theoretically incorrect time.
                            OrderCount = (uint)orderRecords.Count,
                            Price = order.Price,
                            Quantity = (uint)orderRecords.Sum(x => x.Quantity),
                            UpdateType = IncrementalOrderbookUpdateType.New,
                            IncrementalOrderbookUpdateEntries = new List<IncrementalOrderbookUpdateEntry>(orderRecords.Select(o => new IncrementalOrderbookUpdateEntry(o.OrderId, o.Quantity, o.TheoreticalQueuePosition))),
                        };
                        return Task.FromResult(ExchangeResult.Null);
                    }
                }
            }
            else return Task.FromResult(new ExchangeResult(RejectionCreator.GenerateOrderCoreReject(order, RejectionReason.OrderbookNotFound)));
        }

        public Task<ExchangeResult> ProcessOrderAsync(ModifyOrder modifyOrder)
        {
            DateTime now = DateTime.UtcNow;
            _textLogger.Information(nameof(TradingEngineServer), $"Handling ModifyOrder: {modifyOrder}");
            if (_exchange.TryGetOrderbook(modifyOrder.SecurityId, out var orderbook))
            {
                if (RejectGenerator.TryRejectModifyOrder(modifyOrder, orderbook, out var rejection))
                {
                    return Task.FromResult(new ExchangeResult(rejection));
                }
                else
                {
                    orderbook.ChangeOrder(modifyOrder);
                    if (orderbook.TryMatch(out MatchResult matchResults))
                    {
                        return Task.FromResult(new ExchangeResult(matchResults.Fills));
                    }
                    else
                    {
                        var orderRecords = orderbook.GetOrderbookSlice(modifyOrder.Price).OrderRecords;
                        var orderLevel = new IncrementalOrderbookUpdate
                        {
                            SecurityId = modifyOrder.SecurityId,
                            EntryType = modifyOrder.IsBuySide ? OrderbookEntryType.Bid : OrderbookEntryType.Ask,
                            EventTime = now, // Theoretically incorrect time.
                            OrderCount = (uint)orderRecords.Count,
                            Price = modifyOrder.Price,
                            Quantity = (uint)orderRecords.Sum(x => x.Quantity),
                            UpdateType = IncrementalOrderbookUpdateType.Change,
                            IncrementalOrderbookUpdateEntries = new List<IncrementalOrderbookUpdateEntry>(orderRecords.Select(o => new IncrementalOrderbookUpdateEntry(o.OrderId, o.Quantity, o.TheoreticalQueuePosition))),
                        };

                        // send market data.

                        return Task.FromResult(ExchangeResult.Null);
                    }

                }
            }
            else return Task.FromResult(new ExchangeResult(RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.OrderbookNotFound)));
        }

        public Task<ExchangeResult> ProcessOrderAsync(CancelOrder cancelOrder)
        {
            DateTime now = DateTime.UtcNow;
            _textLogger.Information(nameof(TradingEngineServer), $"Handling CancelOrder: {cancelOrder}");
            if (_exchange.TryGetOrderbook(cancelOrder.SecurityId, out var orderbook))
            { 
                if (RejectGenerator.TryRejectCancelOrder(cancelOrder, orderbook, out var rejection))
                {
                    return Task.FromResult(new ExchangeResult(rejection));
                }
                else
                {
                    if (orderbook.TryGetOrder(cancelOrder.OrderId, out OrderRecord orderRecord))
                    {
                        orderbook.RemoveOrder(cancelOrder);
                        var orderRecords = orderbook.GetOrderbookSlice(orderRecord.Price).OrderRecords;
                        var orderLevel = new IncrementalOrderbookUpdate
                        {
                            SecurityId = cancelOrder.SecurityId,
                            EntryType = orderRecord.IsBuySide ? OrderbookEntryType.Bid : OrderbookEntryType.Ask,
                            EventTime = now, // Theoretically incorrect time.
                            OrderCount = (uint)orderRecords.Count,
                            Price = orderRecord.Price,
                            Quantity = (uint)orderRecords.Sum(x => x.Quantity),
                            UpdateType = orderRecords.Count == 0 ? IncrementalOrderbookUpdateType.Delete : IncrementalOrderbookUpdateType.Change,
                            IncrementalOrderbookUpdateEntries = new List<IncrementalOrderbookUpdateEntry>(orderRecords.Select(o => new IncrementalOrderbookUpdateEntry(o.OrderId, o.Quantity, o.TheoreticalQueuePosition))),
                        };
                    }
                    else
                    {
                        return Task.FromResult(new ExchangeResult(RejectionCreator.GenerateOrderCoreReject(cancelOrder, RejectionReason.OrderNotFound)));
                    }
                    return Task.FromResult(ExchangeResult.Null);
                }
            }
            else return Task.FromResult(new ExchangeResult(RejectionCreator.GenerateOrderCoreReject(cancelOrder, RejectionReason.OrderbookNotFound)));
        }

        public Task CancelAllAsync(List<IOrderStatus> orderStatuses)
        {
            _textLogger.Information(nameof(TradingEngineServer), $"Unsolicited cancel for all active orders: {string.Join(", ", orderStatuses.Select(x => x.OrderId))}");
            var cancelTasks = orderStatuses.Select(x =>
            {
                return ProcessOrderAsync(new CancelOrder(x));
            });
            return Task.WhenAll(cancelTasks);
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _textLogger.Information(nameof(TradingEngineServer), $"Starting Trading Engine");
            using EdenOrderEntryServer oeServer = new EdenOrderEntryServer(this, _engineConfiguration.OrderEntryServer.Port);
            oeServer.Start();
            return Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
