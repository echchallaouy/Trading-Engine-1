using Eden.Proto.OrderEntry;
using Grpc.Core;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.OrderEntryProtoAdapter;
using TradingEngineServer.Orders;
using TradingEngineServer.Orders.OrderStatuses;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.OrderEntryCommunicationServer
{
    public class OrderEntryServerClient
    {
        public OrderEntryServerClient(string clientUri, string username, IServerStreamWriter<OrderEntryResponse> responseStream, CancellationToken updateToken)
        {
            if (string.IsNullOrWhiteSpace(clientUri)) throw new ArgumentException("Invalid Uri.", nameof(clientUri));
            Uri = clientUri;
            Username = username;
            _responseStream = responseStream ?? throw new ArgumentNullException(nameof(responseStream));
            _updateToken = updateToken;
        }

        public async Task PublishAcknowledgementAsync(ModifyOrderStatus modifyOrderStatus, CancellationToken token)
        {
            try
            {
                using var updateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _updateToken);
                var orderEntryResponse = new OrderEntryResponse()
                {
                    ModifyOrderAcknowledgement = ProtoAdapter.ModifyOrderStatusToProto(modifyOrderStatus),
                };
                await SendUpdateAsync(orderEntryResponse, updateTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                SetClientException(e);
            }
        }

        public async Task PublishAcknowledgementAsync(NewOrderStatus newOrderStatus, CancellationToken token)
        {
            {
                using var lck = await _activeOrdersLock.WriterLockAsync().ConfigureAwait(false);
                _activeOrders.Add(newOrderStatus);
            }

            try
            {
                using var updateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _updateToken);
                var orderEntryResponse = new OrderEntryResponse()
                {
                    NewOrderAcknowledgement = ProtoAdapter.NewOrderStatusToProto(newOrderStatus),
                };
                await SendUpdateAsync(orderEntryResponse, updateTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                SetClientException(e);
            }
        }

        public async Task PublishAcknowledgementAsync(CancelOrderStatus cancelOrderStatus, CancellationToken token)
        {
            {
                using var lck = await _activeOrdersLock.WriterLockAsync().ConfigureAwait(false);
                _activeOrders.RemoveAll(x => x.OrderId == cancelOrderStatus.OrderId);
            }

            try
            {
                using var updateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _updateToken);
                var orderEntryResponse = new OrderEntryResponse()
                {
                    CancelOrderAcknowledgement = ProtoAdapter.CancelOrderStatusToProto(cancelOrderStatus),
                };
                await SendUpdateAsync(orderEntryResponse, updateTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                SetClientException(e);
            }
        }

        public async Task PublishFillAsync(Fill fill, CancellationToken token)
        {
            {
                using var lck = await _activeOrdersLock.WriterLockAsync().ConfigureAwait(false);
                if (fill.IsCompleteFill)
                    _activeOrders.RemoveAll(x => x.OrderId == fill.OrderBase.OrderId);
            }

            try
            {
                using var updateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _updateToken);
                var orderEntryResponse = new OrderEntryResponse()
                {
                    Fill = ProtoAdapter.FillToProto(fill),
                };
                await SendUpdateAsync(orderEntryResponse, updateTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                SetClientException(e);
            }
        }

        public async Task PublishRejectAsync(Rejection rejection, CancellationToken token)
        {
            try
            {
                using var updateTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _updateToken);
                var orderEntryResponse = new OrderEntryResponse()
                {
                    Rejection = ProtoAdapter.RejectionToProto(rejection),
                };
                await SendUpdateAsync(orderEntryResponse, updateTokenSource.Token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                SetClientException(e);
            }
        }

        public async Task<List<IOrderStatus>> GetActiveOrders(CancellationToken token)
        {
            using var lck = await _activeOrdersLock.ReaderLockAsync(token).ConfigureAwait(false);
            return new List<IOrderStatus>(_activeOrders);
        }

        public async Task WaitForClientExceptionAsync(CancellationToken token)
        {
            await _clientExceptionSetEvent.WaitAsync(token).ConfigureAwait(false);
            throw _clientException;
        }

        private void SetClientException(Exception exception)
        {
            Interlocked.CompareExchange(ref _clientException, null, exception);
            _clientExceptionSetEvent.Set();
        }

        private async Task SendUpdateAsync(OrderEntryResponse response, CancellationToken token)
        {
            using (await _responseStreamLock.LockAsync(token).ConfigureAwait(false))
            {
                await _responseStream.WriteAsync(response).ConfigureAwait(false);
            }
        }


        public string Uri { get; init; }
        public string Username { get; init; }

        private readonly IServerStreamWriter<OrderEntryResponse> _responseStream;
        private readonly CancellationToken _updateToken = default;
        private readonly AsyncLock _responseStreamLock = new AsyncLock();
        private readonly List<IOrderStatus> _activeOrders = new List<IOrderStatus>();
        private readonly AsyncReaderWriterLock _activeOrdersLock = new AsyncReaderWriterLock();
        private readonly AsyncManualResetEvent _clientExceptionSetEvent = new AsyncManualResetEvent(false);
        private Exception _clientException = null;
    }
}
