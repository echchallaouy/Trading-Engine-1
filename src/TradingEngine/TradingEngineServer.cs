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
using TradingEngineServer.Server;
using TradingEngineServer.Orders;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Rejects;

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
            _textLogger.Information(nameof(TradingEngineServer), $"Handling NewOrder: {order}");
            if (_exchange.TryGetOrderbook(order.SecurityId, out var orderbook))
            {
                if (RejectGenerator.TryRejectNewOrder(order, orderbook, out var rejection))
                {
                    return Task.FromResult(ExchangeResult.CreateExchangeResult(rejection));
                }
                else
                {
                    orderbook.AddOrder(order);
                    var matchResults = orderbook.Match();
                    return Task.FromResult(ExchangeResult.CreateExchangeResult(matchResults.Fills));
                }
            }
            else return Task.FromResult(ExchangeResult.CreateExchangeResult(RejectionCreator.GenerateOrderCoreReject(order, RejectionReason.OrderbookNotFound)));
        }

        public Task<ExchangeResult> ProcessOrderAsync(ModifyOrder modifyOrder)
        {
            _textLogger.Information(nameof(TradingEngineServer), $"Handling ModifyOrder: {modifyOrder}");
            if (_exchange.TryGetOrderbook(modifyOrder.SecurityId, out var orderbook))
            {
                if (RejectGenerator.TryRejectModifyOrder(modifyOrder, orderbook, out var rejection))
                {
                    return Task.FromResult(ExchangeResult.CreateExchangeResult(rejection));
                }
                else
                {
                    orderbook.ChangeOrder(modifyOrder);
                    var matchResults = orderbook.Match();
                    return Task.FromResult(ExchangeResult.CreateExchangeResult(matchResults.Fills));
                }
            }
            else return Task.FromResult(ExchangeResult.CreateExchangeResult(RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.OrderbookNotFound)));
        }

        public Task<ExchangeResult> ProcessOrderAsync(CancelOrder cancelOrder)
        {
            _textLogger.Information(nameof(TradingEngineServer), $"Handling CancelOrder: {cancelOrder}");
            if (_exchange.TryGetOrderbook(cancelOrder.SecurityId, out var orderbook))
            { 
                if (RejectGenerator.TryRejectCancelOrder(cancelOrder, orderbook, out var rejection))
                {
                    return Task.FromResult(ExchangeResult.CreateExchangeResult(rejection));
                }
                else
                {
                    orderbook.RemoveOrder(cancelOrder);
                    return Task.FromResult(ExchangeResult.CreateExchangeResult());
                }
            }
            else return Task.FromResult(ExchangeResult.CreateExchangeResult(RejectionCreator.GenerateOrderCoreReject(cancelOrder, RejectionReason.OrderbookNotFound)));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _textLogger.Information(nameof(TradingEngineServer), $"Starting Trading Engine");
            while (!stoppingToken.IsCancellationRequested)
            {

            }
            _textLogger.Information(nameof(TradingEngineServer), $"Stopping Trading Engine");
            return Task.CompletedTask;
        }
    }
}
