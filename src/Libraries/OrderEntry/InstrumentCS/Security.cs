using System;

namespace TradingEngineServer.Instrument
{
    public class Security
    {
        public int SecurityId { get; set; }
        public string Symbol { get; set; }
        public AllocationAlgorithm AllocationAlgorithm { get; set; }
    }
}
