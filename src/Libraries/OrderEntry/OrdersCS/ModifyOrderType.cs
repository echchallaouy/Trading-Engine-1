using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public enum ModifyOrderType
    {
        Unknown,
        NoChange,
        Price,
        Quantity,
        PriceAndQuantity,
    }
}
