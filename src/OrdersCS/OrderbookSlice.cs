using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orders
{
    public record OrderbookSlice(Side Side, long Price, List<OrderRecord> OrderRecords);
}
