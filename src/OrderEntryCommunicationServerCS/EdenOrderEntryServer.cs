using Eden.Proto.OrderEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Orders;
using TradingEngineServer.OrderEntryProtoAdapter;

namespace TradingEngineServer.OrderEntryCommunicationServer
{
    public class EdenOrderEntryServer : OrderEntryServerBase
    {
        public EdenOrderEntryServer(ITradingUpdateProcessor tradingUpdateProcessor, int port) : base(port)
        {
            _updateProcessor = tradingUpdateProcessor;
        }

        protected override async Task ProcessSubscribeRequestAsync(OrderEntryRequest requestStream, OrderEntryServerClient originalClient, 
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            switch (requestStream.OrderEntryTypeCase)
            {
                case OrderEntryRequest.OrderEntryTypeOneofCase.NewOrder:
                    await ProcessNewOrder(ProtoAdapter.NewOrderFromProto(requestStream.NewOrder), originalClient, clientStore, token).ConfigureAwait(false);
                    break;
                case OrderEntryRequest.OrderEntryTypeOneofCase.ModifyOrder:
                    await ProcessModifyOrder(ProtoAdapter.ModifyOrderFromProto(requestStream.ModifyOrder), originalClient, clientStore, token).ConfigureAwait(false);
                    break;
                case OrderEntryRequest.OrderEntryTypeOneofCase.CancelOrder:
                    await ProcessCancelOrder(ProtoAdapter.CancelOrderFromProto(requestStream.CancelOrder), originalClient, clientStore, token).ConfigureAwait(false);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessCancelOrder(CancelOrder cancelOrder, OrderEntryServerClient client, 
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            await client.PublishAcknowledgementAsync(OrderStatusCreator.GenerateOrderCancelStatus(cancelOrder), token).ConfigureAwait(false);
            var results = await _updateProcessor.ProcessOrderAsync(cancelOrder).ConfigureAwait(false);
            await HandleResults(client, clientStore, results, token).ConfigureAwait(false);
        }

        private async Task ProcessModifyOrder(ModifyOrder modifyOrder, OrderEntryServerClient client, 
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            await client.PublishAcknowledgementAsync(OrderStatusCreator.GenerateModifyOrderStatus(modifyOrder), token).ConfigureAwait(false);
            var results = await _updateProcessor.ProcessOrderAsync(modifyOrder).ConfigureAwait(false);
            await HandleResults(client, clientStore, results, token).ConfigureAwait(false);
        }

        private async Task ProcessNewOrder(Order order, OrderEntryServerClient client, 
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            await client.PublishAcknowledgementAsync(OrderStatusCreator.GenerateNewOrderStatus(order), token).ConfigureAwait(false);
            var results = await _updateProcessor.ProcessOrderAsync(order).ConfigureAwait(false);
            await HandleResults(client, clientStore, results, token).ConfigureAwait(false);
        }

        private static Task HandleResults(OrderEntryServerClient client, ICache<string, OrderEntryServerClient> clientStore,
            Orderbook.ExchangeResult results, CancellationToken token)
        {
            switch (results.ExchangeInformationType)
            {
                case Orderbook.ExchangeInformationType.Rejection:
                    return HandleRejection(client, results.Rejection, token);
                case Orderbook.ExchangeInformationType.Fill:
                    return HandleFill(clientStore, results.Fills, token);
                default:
                    throw new InvalidOperationException($"Unknown ExchangeInformationType ({results.ExchangeInformationType})");
            }
        }

        private static Task HandleFill(ICache<string, OrderEntryServerClient> clientStore, List<Fill> fills, CancellationToken token)
        {
            var publishFillTasks = fills.Select(f =>
            {
                if (clientStore.TryGet(f.OrderBase.Username, out var client))
                {
                    return client.PublishFillAsync(f, token);
                }
                return Task.CompletedTask;
            });
            return Task.WhenAll(publishFillTasks);
        }

        private static Task HandleRejection(OrderEntryServerClient client, Rejects.Rejection rejection, CancellationToken token)
        {
            return client.PublishRejectAsync(rejection, token);
        }

        protected override async Task HandleClientDisconnectAsync(OrderEntryServerClient client)
        {
            await _updateProcessor.CancelAllAsync(await client.GetActiveOrders(default));
        }

        private readonly ITradingUpdateProcessor _updateProcessor;
    }
}
