using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    /// <summary>
    /// Two fills belonging to the same trade are considered equal.
    /// </summary>
    public sealed class TradeFillComparer : IEqualityComparer<Fill>
    {
        public static TradeFillComparer Comparer { get; } = new TradeFillComparer();

        public bool Equals(Fill x, Fill y)
        {
            return x.ExecutionId == y.ExecutionId;
        }

        public int GetHashCode(Fill obj)
        {
            return obj.ExecutionId.GetHashCode();
        }
    }
}
