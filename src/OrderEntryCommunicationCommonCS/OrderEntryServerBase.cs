using Eden.Proto.OrderEntry;
using Grpc.Core;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Fills;
using TradingEngineServer.Rejects;


namespace TradingEngineServer.OrderEntryCommunication
{
    public class OrderEntryServerBase : OrderEntryService.OrderEntryServiceBase, IOrderEntryServer
    {
        public OrderEntryServerBase(int port) 
        {
            Port = port;
            // https://github.com/grpc/grpc/blob/1d91362f8124751ecfc1929df207006cabb41dae/include/grpc/impl/codegen/grpc_types.h#L136
            _server = new Grpc.Core.Server(new List<ChannelOption>
            {
                new ChannelOption("grpc.keepalive_time_ms", 1000),
                new ChannelOption("grpc.keepalive_timeout_ms", 1000),
                new ChannelOption("grpc.keepalive_permit_without_calls", 1),
                new ChannelOption("grpc.http2.max_pings_without_data", 0),
                new ChannelOption("grpc.http2.min_time_between_pings_ms", 1000),
            });
            _ = ShutdownAsync(_shutdownStartEvent, _shutdownOverEvent, _server);
        }

        public List<OrderEntryServerClient> GetClients()
        {
            return new List<OrderEntryServerClient>(_clientStore.GetAll());
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

        public void Start()
        {
            if (!_started)
            {
                try
                {
                    _server.Services.Add(OrderEntryService.BindService(this));
                    _server.Ports.Add(new ServerPort("0.0.0.0", Port, ServerCredentials.Insecure));
                    _server.Start();
                    _started = true;
                }
                catch (Exception)
                {
                    _started = false;
                }
            }
        }

        public async Task StopAsync()
        {
            if (_started)
            {
                _shutdownStartEvent.TrySetResult(null);
                await _shutdownOverEvent.Task.ConfigureAwait(false);
            }
        }

        private static async Task ShutdownAsync(TaskCompletionSource<object> shutdownStartEvent, 
            TaskCompletionSource<object> shutdownOverEvent, Grpc.Core.Server server)
        {
            await shutdownStartEvent.Task.ConfigureAwait(false);
            await server.KillAsync().ConfigureAwait(false);
            server = null;
            shutdownOverEvent.TrySetResult(null);
        }

        ~OrderEntryServerBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (_disposeLock)
            {
                if (_disposed)
                    return;
                _disposed = true;
            }

            if (disposing)
            {
                // Dipose unmanaged resources
            }

            // Dipose managed resources.
            _shutdownStartEvent.TrySetResult(null);
        }

        public int Port { get; init; }

        private readonly Grpc.Core.Server _server;
        private readonly ServerClientStore _clientStore = new ServerClientStore();
        private bool _started = false;
        private bool _disposed = false;
        private readonly TaskCompletionSource<object> _shutdownStartEvent = new TaskCompletionSource<object>();
        private readonly TaskCompletionSource<object> _shutdownOverEvent = new TaskCompletionSource<object>();
        private readonly object _disposeLock = new object();
    }
}
