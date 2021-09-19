using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using TradingEngineServer.Rejects;

namespace TradingEngineServer.OrderEntryCommunication
{
    public interface IOrderEntryOutbound
    {

    }

    public interface IOrderEntryInbound
    {
        
    }

    public interface IOrderEntryServer : IOrderEntryInbound, IOrderEntryOutbound, IDisposable
    {
        void Start();
        Task StopAsync();
        List<OrderEntryServerClient> GetClients();
    }
}
