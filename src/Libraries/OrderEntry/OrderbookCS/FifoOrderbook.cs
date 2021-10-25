using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingEngineServer.Orderbook.MatchingAlgorithm;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Orderbook
{
    public interface IFifoOrderbook
    {
    }

    public class FifoOrderbook : MatchingOrderbook, IFifoOrderbook
    {
        public FifoOrderbook(IRetrievalOrderbook ob) : base(ob, FifoMatchingAlgorithm.MatchingAlgorithm)
        { }
    }
}
