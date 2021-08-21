﻿using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using TradingEngineServer.Fills;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orderbook;
using TradingEngineServer.Orders;

namespace TradingEngineServer.Exchange
{
    public class TradingExchange : ITradingExchange
    {
        public TradingExchange(IOptions<TradingExchangeConfiguration> exchangeConfiguration)
        {
            var ec = exchangeConfiguration.Value ?? throw new ArgumentNullException(nameof(exchangeConfiguration));
            _exchangeId = ec.ExchangeId;
            _exchangeName = ec.ExchangeName;
            foreach (var security in ec.Securities)
                _orderbooks.Add(security, OrderbookFactory.CreateOrderbook(security, security.AllocationAlgorithm));
        }

        // IExchange // 
        public int GetExchangeId()
        {
            return _exchangeId;
        }

        public string GetExchangeName()
        {
            return _exchangeName;
        }

        public bool TryGetOrderbook(Security security, out IMatchingOrderbook orderbook)
        {
            return _orderbooks.TryGetValue(security, out orderbook);
        }

        private readonly int _exchangeId;
        private readonly string _exchangeName;
        private readonly Dictionary<Security, IMatchingOrderbook> _orderbooks = 
            new Dictionary<Security, IMatchingOrderbook>(SecurityComparer.Comparer);
    }
}
