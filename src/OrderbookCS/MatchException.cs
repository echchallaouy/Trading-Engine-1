using System;
namespace TradingEngineServer.Orderbook
{
    public class MatchException : Exception
    {
        public MatchException()
        {
        }

        public MatchException(string message) : base(message) { }

        public MatchException(string message, Exception innerException) : base(message, innerException) { }
    }
}
