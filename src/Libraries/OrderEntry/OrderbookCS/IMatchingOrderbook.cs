using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orderbook
{
    public interface IMatchingOrderbook : IOrderEntryOrderbook
    {
        bool CanMatch();
        MatchResult Match();
        bool TryMatch(out MatchResult matchResult);
    }
}
