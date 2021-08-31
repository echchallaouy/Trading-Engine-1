using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradingEngineServer.Fills;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.OrderEntryCommunication
{
    public interface IOrderEntryOutbound
    {

    }

    public interface IOrderEntryInbound
    {
        
    }

    public interface IOrderEntryServer : IOrderEntryInbound, IOrderEntryOutbound
    {
        Task<IReadOnlyList<OrderEntryServerClient>> GetClientsAsync(CancellationToken token);
    }
}
