using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using TradingEngineServer.OrderbookData;
using TradingEngineServer.Orders;
using TradingEngineServer.Trades;

namespace TradingEngineServer.Orderbook
{
    public class MatchResult
    {
        public MatchResult()
        { }

        public void AddTradeResult(TradeResult tradeResult)
        {
            AddTrade(tradeResult.Trade);
            AddFill(tradeResult.BuyFill);
            AddFill(tradeResult.SellFill);
        }

        public void AddOrderbookUpdateResult(OrderbookUpdateResult orderbookResult)
        {
            AddIncrementalOrderbookUpdate(orderbookResult.IncrementalOrderbookUpdate);
        }

        public void AddIncrementalOrderbookUpdate(IncrementalOrderbookUpdate iou)
        {
            _orderbookUpdates.Add(iou);
        }

        public void AddTrade(Trade trade)
        {
            _trades.Add(trade);
        }

        public void AddFill(Fill fill)
        {
            _fills.Add(fill);
        }

        public List<Trade> Trades
        {
            get
            {
                return new List<Trade>(_trades);
            }
        }

        public List<Fill> Fills
        {
            get
            {
                return new List<Fill>(_fills);
            }
        }

        public List<IncrementalOrderbookUpdate> IncrementalOrderbookUpdates
        {
            get
            {
                return new List<IncrementalOrderbookUpdate>(_orderbookUpdates);
            }
        }

        private readonly List<Fill> _fills = new List<Fill>();
        private readonly List<Trade> _trades = new List<Trade>();
        private readonly List<IncrementalOrderbookUpdate> _orderbookUpdates = new List<IncrementalOrderbookUpdate>();
    }
}
