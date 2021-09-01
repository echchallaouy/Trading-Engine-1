using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders.OrderStatuses
{
    public interface IOrderStatus : IOrderCore
    {
        DateTime CreationTime { get; }
    }

    public interface IModifyOrderStatus : IOrderStatus
    {

    }

    public interface INewOrderStatus : IOrderStatus
    {

    }

    public interface ICancelOrderStatus : IOrderStatus
    {

    }

}
