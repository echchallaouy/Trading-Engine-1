using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Instrument
{
    public class SecurityComparer : IEqualityComparer<Security>
    {
        public static IEqualityComparer<Security> Comparer { get; } = new SecurityComparer();

        public bool Equals(Security x, Security y)
        {
            return x.SecurityId == y.SecurityId;
        }

        public int GetHashCode(Security obj)
        {
            return obj.SecurityId.GetHashCode();
        }
    }
}
