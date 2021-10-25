using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingEngineServer.Orderbook.MatchingAlgorithm;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public interface ILifoOrderbook
    {
    }

    public class LifoOrderbook : MatchingOrderbook, ILifoOrderbook
    {
        public LifoOrderbook(IRetrievalOrderbook ob) : base(ob, LifoMatchingAlgorithm.MatchingAlgorithm)
        { }
    }
}
