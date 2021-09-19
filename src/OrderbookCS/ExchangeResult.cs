using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.Orderbook
{
    public enum ExchangeInformationType
    {
        None,
        Rejection,
        Fill,
    }

    public sealed class ExchangeResult
    {
        public static ExchangeResult CreateExchangeResult(Rejection rejection)
        {
            return new ExchangeResult()
            {
                ExchangeInformationType = ExchangeInformationType.Rejection,
                Rejection = rejection,
            };
        }

        public static ExchangeResult CreateExchangeResult(List<Fill> fills)
        {
            return new ExchangeResult()
            {
                ExchangeInformationType = ExchangeInformationType.Fill,
                Fills = fills,
            };
        }

        public static ExchangeResult CreateExchangeResult()
        {
            return new ExchangeResult()
            {
                ExchangeInformationType = ExchangeInformationType.None,
            };
        }

        // PRIVATE CONSTRUCTOR //
        private ExchangeResult()
        { }

        // FIELDS // 
        public ExchangeInformationType ExchangeInformationType { get; private set; }
        public List<Fill> Fills { get; private set; }
        public Rejection Rejection { get; set; }
    }
}
