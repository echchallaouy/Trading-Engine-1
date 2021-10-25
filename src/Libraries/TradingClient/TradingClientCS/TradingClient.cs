using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TradingClient.Core
{
    public class TradingClient : ITradingClient
    {
        public List<ClientFill> GetFills()
        {
            throw new NotImplementedException();
        }

        public List<ClientPosition> GetOutstandingPositions()
        {
            throw new NotImplementedException();
        }

        public List<ClientTrade> GetTrades()
        {
            throw new NotImplementedException();
        }

        public bool IsOnline()
        {
            throw new NotImplementedException();
        }

        public void StartTradingClient()
        {
            throw new NotImplementedException();
        }

        public void StopTradingClient()
        {
            throw new NotImplementedException();
        }

        public Task SubmitCancelOrderAsync(CancelOrderRequest orderCancelRequest)
        {
            throw new NotImplementedException();
        }

        public Task SubmitModifyOrderAsync(ModifyOrderRequest orderModifyRequest)
        {
            throw new NotImplementedException();
        }

        public Task SubmitNewOrderAsync(NewOrderRequest newOrderRequest)
        {
            throw new NotImplementedException();
        }
    }
}
