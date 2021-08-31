using Eden.Proto.OrderEntry;
using Grpc.Core;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Fills;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.OrderEntryCommunication
{
    public class OrderEntryServerBase : OrderEntryService.OrderEntryServiceBase, IOrderEntryServer
    {
        public OrderEntryServerBase() 
        { }

        public async Task<IReadOnlyList<OrderEntryServerClient>> GetClientsAsync(CancellationToken token)
        {
            return new ReadOnlyCollection<OrderEntryServerClient>(_clientStore.GetAll());
        }

        public sealed override async Task OrderEntry(IAsyncStreamReader<OrderEntryRequest> requestStream, IServerStreamWriter<OrderEntryResponse> responseStream, ServerCallContext context)
        {
            var username = UsernameGenerator.GenerateRandomUsername(10);
            var client = new OrderEntryServerClient(context.Peer, username, responseStream, context.CancellationToken);
            try
            {
                _clientStore.Add(username, client);

                try
                {
                    using var doneOrderEntryTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
                    using Task waitForErrorTask = client.WaitForClientExceptionAsync(doneOrderEntryTokenSource.Token);
                    using Task processRequestTask = ProcessSubscribeRequestAsync(requestStream.Current, username, _clientStore, doneOrderEntryTokenSource.Token);
                    Task[] tasks = new[] { waitForErrorTask, processRequestTask };
                    Task firstFinishedTask = await Task.WhenAny(tasks).ConfigureAwait(false);
                    doneOrderEntryTokenSource.Cancel();
                    try
                    {
                        await Task.WhenAll(tasks).ConfigureAwait(false);
                    }
                    catch { }
                    await firstFinishedTask.ConfigureAwait(false);
                }
                finally
                {
                    _clientStore.Remove(username);
                }
            }
            finally
            {
                await HandleClientDisconnectAsync(client).ConfigureAwait(false);
                await FinalizeClientAsync(client).ConfigureAwait(false);
            }
        }

        // PROTECTED //
        private async Task ReadClientSubscriptionRequestsAsync(IAsyncStreamReader<OrderEntryRequest> requestStream, string username,
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            while (await requestStream.MoveNext(token).ConfigureAwait(false))
            {
                await ProcessSubscribeRequestAsync(requestStream.Current, username, clientStore, token).ConfigureAwait(false);
            }
        }

        protected virtual Task ProcessSubscribeRequestAsync(OrderEntryRequest requestStream, string username,
            ICache<string, OrderEntryServerClient> clientStore, CancellationToken token)
        {
            return Task.CompletedTask;
        }

        protected virtual Task HandleClientDisconnectAsync(OrderEntryServerClient client)
        {
            return Task.CompletedTask;
        }

        protected virtual Task FinalizeClientAsync(OrderEntryServerClient client)
        {
            return Task.CompletedTask;
        }

        private readonly ServerClientStore _clientStore = new ServerClientStore();
        private readonly List<OrderEntryServerClient> _clients = new List<OrderEntryServerClient>();
    }
}
