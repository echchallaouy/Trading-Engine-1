using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TradingClient.Core
{
    public interface ITradingClient
    {
        void StartTradingClient();
        void StopTradingClient();
        bool IsOnline();
        List<ClientPosition> GetOutstandingPositions();
        List<ClientFill> GetFills();
        List<ClientTrade> GetTrades();
        Task SubmitNewOrderAsync(NewOrderRequest newOrderRequest);
        Task SubmitModifyOrderAsync(ModifyOrderRequest orderModifyRequest);
        Task SubmitCancelOrderAsync(CancelOrderRequest orderCancelRequest);
    }
}
