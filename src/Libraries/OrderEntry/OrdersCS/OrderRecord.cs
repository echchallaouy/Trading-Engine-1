using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    /// <summary>
    /// Read-only representation of an order.
    /// </summary>
    public record OrderRecord(long OrderId, long Price, uint Quantity, bool IsBuySide, string Username, int SecurityId, uint TheoreticalQueuePosition);
}
