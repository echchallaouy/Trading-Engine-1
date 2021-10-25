using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orderbook.MatchingAlgorithm;

namespace TradingEngineServer.Orderbook
{
    public interface IProRataOrderbook
    {

    }

    class ProRataOrderbook : MatchingOrderbook, IProRataOrderbook
    {
        public ProRataOrderbook(IRetrievalOrderbook ob) : base (ob, ProRataMatchingAlgorithm.MatchingAlgorithm)
        {

        }
    }
}
