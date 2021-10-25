using System;
using System.Collections.Generic;
using System.Text;

using TradingEngineServer.Instrument;

namespace TradingEngineServer.Orderbook
{
    public class OrderbookFactory
    {
        public static MatchingOrderbook CreateOrderbook(IRetrievalOrderbook ob, AllocationAlgorithm fillAllocationAlgorithm)
        {
            return fillAllocationAlgorithm switch
            {
                AllocationAlgorithm.Fifo => new FifoOrderbook(ob),
                AllocationAlgorithm.Lifo => new LifoOrderbook(ob),
                AllocationAlgorithm.ProRata => new ProRataOrderbook(ob),
                _ => throw new InvalidOperationException($"Unknown FillAllocationAlgorithm ({fillAllocationAlgorithm})"),
            };
        }

        public static MatchingOrderbook CreateOrderbook(Security inst)
        {
            var retrievalOrderbook = new Orderbook(inst);
            return CreateOrderbook(retrievalOrderbook, inst.AllocationAlgorithm);
        }
    }
}
