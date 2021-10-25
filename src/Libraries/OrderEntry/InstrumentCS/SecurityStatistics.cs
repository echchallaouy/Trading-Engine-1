using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Instrument
{
    public class SecurityStatistics
    {
        public SecurityStatistics(Security security)
        {
            // PROPERTIES //
            Security = security;
            Volume = 0;
            Last = 0;
            Low = long.MaxValue;
            High = long.MinValue;
        }

        public void AcceptPrice(long lastPrice)
        {
            Last = lastPrice;
            if (lastPrice > High)
            {
                High = lastPrice;
            }
            else if (lastPrice < Low)
            {
                Low = lastPrice;
            }
        }

        public void AddVolume(uint quantity)
        {
            Volume += quantity; 
        }

        // PROPERTIES //
        public uint Volume { get; private set; }
        public long Last { get; private set; }
        public long Low { get; private set; }
        public long High { get; private set; }
        public Security Security { get; private set; }
    }
}
