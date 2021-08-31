using Eden.Proto.OrderEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Orders;
using TradingEngineServer.Orders.OrderStatuses;

namespace TradingEngineServer.OrderEntryCommunication
{
    public class EdenOrderEntryServer : OrderEntryServerBase
    {
        public EdenOrderEntryServer(ITradingUpdateProcessor tradingUpdateProcessor) : base()
        {
            _updateProcessor = tradingUpdateProcessor;
        }

        protected override async Task ProcessSubscribeRequestAsync(OrderEntryRequest requestStream, string username, ICache<string, OrderEntryServerClient>
             clientStore, CancellationToken token)
        {
            var client = clientStore.Get(username);
            switch (requestStream.OrderEntryTypeCase)
            {
                case OrderEntryRequest.OrderEntryTypeOneofCase.NewOrder:
                    await ProcessNewOrder(ProtoAdapter.NewOrderFromProto(requestStream.NewOrder), client, clientStore, token).ConfigureAwait(false);
                    break;
                case OrderEntryRequest.OrderEntryTypeOneofCase.ModifyOrder:
                    await ProcessModifyOrder(ProtoAdapter.ModifyOrderFromProto(requestStream.ModifyOrder), client, clientStore, token).ConfigureAwait(false);
                    break;
                case OrderEntryRequest.OrderEntryTypeOneofCase.CancelOrder:
                    await ProcessCancelOrder(ProtoAdapter.CancelOrderFromProto(requestStream.CancelOrder), client, clientStore, token).ConfigureAwait(false);
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

        private static async Task HandleResults(OrderEntryServerClient client, ICache<string, OrderEntryServerClient> clientStore,
            Orderbook.ExchangeResult results, CancellationToken token)
        {
            switch (results.ExchangeInformationType)
            {
                case Orderbook.ExchangeInformationType.Rejection:
                    await client.PublishRejectAsync(results.Rejection, token).ConfigureAwait(false);
                    break;
                case Orderbook.ExchangeInformationType.Fill:
                    {
                        var publishFillTasks = results.Fills.Select(f =>
                        {
                            if (clientStore.TryGet(f.OrderBase.Username, out var fillClient))
                            {
                                return fillClient.PublishFillAsync(f, token);
                            }
                            return Task.CompletedTask;
                        });
                        await Task.WhenAll(publishFillTasks).ConfigureAwait(false);
                    }
                    break;
                default:
                    break;
            }
        }

        protected override async Task HandleClientDisconnectAsync(OrderEntryServerClient client)
        {
            await _updateProcessor.CancelAllAsync(await client.GetActiveOrders(default));
        }

        private readonly ITradingUpdateProcessor _updateProcessor;
    }
}
