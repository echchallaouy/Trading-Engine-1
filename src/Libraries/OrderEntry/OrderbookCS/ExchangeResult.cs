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

    public class ExchangeResult
    {
        public static ExchangeResult Null { get; } = new ExchangeResult();

        public ExchangeResult()
        {
            ExchangeInformationType = ExchangeInformationType.None;
        }

        public ExchangeResult(Rejection rejection)
        {
            ExchangeInformationType = ExchangeInformationType.Rejection;
            Rejection = rejection;
        }

        public ExchangeResult(List<Fill> fills)
        {
            ExchangeInformationType = ExchangeInformationType.Fill;
            Fills = fills;
        }

        // FIELDS // 
        public ExchangeInformationType ExchangeInformationType { get; private set; }
        public List<Fill> Fills { get; private set; }
        public Rejection Rejection { get; private set; }
    }
}
