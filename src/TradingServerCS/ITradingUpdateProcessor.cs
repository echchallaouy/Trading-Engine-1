using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Server
{
    public interface ITradingUpdateProcessor
    {
        Task<ExchangeResult> ProcessOrderAsync(Order order);
        Task<ExchangeResult> ProcessOrderAsync(ModifyOrder modifyOrder);
        Task<ExchangeResult> ProcessOrderAsync(CancelOrder cancelOrder);
    }
}
