using System;
using System.Collections.Generic;
using System.Linq;
using Exodus.Proto.MarketData;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace TradingEngineServer.MarketDataProtoAdapter
{
    public class ProtoAdapter
    {
        private static Timestamp DateTimeToProto(DateTime eventTime)
        {
            return Timestamp.FromDateTime(eventTime);
        }

        private static DateTime DateTimeFromProto(Timestamp timestamp)
        {
            return timestamp.ToDateTime();
        }

        public static Trades.Trade TradeFromProto(Trade proto)
        {
            return new Trades.Trade(DateTimeFromProto(proto.EventTime), proto.SecurityId, proto.Price, proto.Quantity, proto.ExecutionId, TradeOrderIdEntryFromProto(proto.TradeOrderIdEntries));
        }

        public static Trade TradeToProto(Trades.Trade trade)
        {
            var t =  new Trade()
            {
                SecurityId = trade.SecurityId,
                EventTime = DateTimeToProto(trade.EventTime),
                ExecutionId = trade.ExecutionId,
                Price = trade.Price,
                Quantity = trade.Quantity,
            };
            t.TradeOrderIdEntries.AddRange(TradeOrderEntryToProto(trade.TradeOrderIdEntries));
            return t;
        }

        public static Trades.TradeOrderIdEntry TradeOrderIdEntryFromProto(TradeOrderIdEntry proto)
        {
            return new Trades.TradeOrderIdEntry(proto.OrderId, proto.Quantity);
        }

        public static TradeOrderIdEntry TradeOrderIdEntryToProto(Trades.TradeOrderIdEntry tradeEntry)
        {
            return new TradeOrderIdEntry()
            {
                OrderId = tradeEntry.OrderId,
                Quantity = tradeEntry.Quantity,
            };
        }

        public static List<TradeOrderIdEntry> TradeOrderEntryToProto(List<Trades.TradeOrderIdEntry> tradeEntries)
        {
            return new List<TradeOrderIdEntry>(tradeEntries.Select(TradeOrderIdEntryToProto));
        }

        public static List<Trades.TradeOrderIdEntry> TradeOrderIdEntryFromProto(RepeatedField<TradeOrderIdEntry> proto)
        {
            return new List<Trades.TradeOrderIdEntry>(proto.Select(TradeOrderIdEntryFromProto));
        }

        public static OrderbookData.IncrementalOrderbookUpdate IncrementalOrderbookUpdateFromProto(IncrementalOrderbookUpdate iou)
        {
            var i = new OrderbookData.IncrementalOrderbookUpdate()
            {
                EventTime = DateTimeFromProto(iou.EventTime),
                SecurityId = iou.SecurityId,
                OrderCount = iou.OrderCount,
                Price = iou.Price,
                Quantity = iou.Quantity,
                UpdateType = IncrementalorderbookUpdateTypeFromProto(iou.UpdateType),
                EntryType = EntryTypeFromProto(iou.EntryType),
            };
            i.IncrementalOrderbookUpdateEntries = IncrementalOrderbookUpdateEntriesFromProto(iou.IncrementalOrderbookUpdateEntries);
            return i;
        }

        private static List<OrderbookData.IncrementalOrderbookUpdateEntry> IncrementalOrderbookUpdateEntriesFromProto(RepeatedField<IncrementalOrderbookUpdateEntry> ioue)
        {
            return new List<OrderbookData.IncrementalOrderbookUpdateEntry>(ioue.Select(IncrementalOrderbookUpdateEntriesFromProto));
        }

        private static OrderbookData.IncrementalOrderbookUpdateEntry IncrementalOrderbookUpdateEntriesFromProto(IncrementalOrderbookUpdateEntry proto)
        {
            return new OrderbookData.IncrementalOrderbookUpdateEntry(proto.OrderId, proto.Quantity, proto.TheoreticalQueuePosition);
        }

        private static OrderbookData.OrderbookEntryType EntryTypeFromProto(IncrementalOrderbookUpdate.Types.OrderbookEntryType entryType)
        {
            return entryType switch
            {
                IncrementalOrderbookUpdate.Types.OrderbookEntryType.Null => OrderbookData.OrderbookEntryType.Null,
                IncrementalOrderbookUpdate.Types.OrderbookEntryType.Ask => OrderbookData.OrderbookEntryType.Ask,
                IncrementalOrderbookUpdate.Types.OrderbookEntryType.Bid => OrderbookData.OrderbookEntryType.Bid,
                _ => throw new InvalidOperationException($"Unknown EntryType ({entryType})"),
            };
        }

        private static OrderbookData.IncrementalOrderbookUpdateType IncrementalorderbookUpdateTypeFromProto(IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType updateType)
        {
            return updateType switch
            {
                IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.Change => OrderbookData.IncrementalOrderbookUpdateType.Change,
                IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.Delete => OrderbookData.IncrementalOrderbookUpdateType.Delete,
                IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.New => OrderbookData.IncrementalOrderbookUpdateType.New,
                _ => throw new InvalidOperationException($"Unknown UpdateType ({updateType})"),
            };
        }

        public static IncrementalOrderbookUpdate IncrementalOrderbookUpdateToProto(OrderbookData.IncrementalOrderbookUpdate iou)
        {
            var i =  new IncrementalOrderbookUpdate
            {
                EventTime = DateTimeToProto(iou.EventTime),
                SecurityId = iou.SecurityId,
                Price = iou.Price,
                Quantity = iou.Quantity,
                OrderCount = iou.OrderCount,
                UpdateType = IncrementalOrderbookUpdateTypeToProto(iou.UpdateType),
                EntryType = EntryTypeToProto(iou.EntryType),
            };
            i.IncrementalOrderbookUpdateEntries.AddRange(IncrementalOrderbookUpdateEntriesToProto(iou.IncrementalOrderbookUpdateEntries));
            return i;
        }

        private static List<IncrementalOrderbookUpdateEntry> IncrementalOrderbookUpdateEntriesToProto(List<OrderbookData.IncrementalOrderbookUpdateEntry> ioue)
        {
            return new List<IncrementalOrderbookUpdateEntry>(ioue.Select(IncrementalOrderbookUpdateEntriesToProto));
        }

        private static IncrementalOrderbookUpdateEntry IncrementalOrderbookUpdateEntriesToProto(OrderbookData.IncrementalOrderbookUpdateEntry ioue)
        {
            return new IncrementalOrderbookUpdateEntry
            {
                OrderId = ioue.OrderId,
                Quantity = ioue.Quantity,
                TheoreticalQueuePosition = ioue.TheoreticalQueuePosition,
            };
        }

        private static IncrementalOrderbookUpdate.Types.OrderbookEntryType EntryTypeToProto(OrderbookData.OrderbookEntryType entryType)
        {
            return entryType switch
            {
                OrderbookData.OrderbookEntryType.Null => IncrementalOrderbookUpdate.Types.OrderbookEntryType.Null,
                OrderbookData.OrderbookEntryType.Ask => IncrementalOrderbookUpdate.Types.OrderbookEntryType.Ask,
                OrderbookData.OrderbookEntryType.Bid => IncrementalOrderbookUpdate.Types.OrderbookEntryType.Bid,
                _ => throw new InvalidOperationException($"Unknown EntryType ({entryType})"),
            };
        }

        private static IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType IncrementalOrderbookUpdateTypeToProto(OrderbookData.IncrementalOrderbookUpdateType updateType)
        {
            return updateType switch
            {
                OrderbookData.IncrementalOrderbookUpdateType.Change => IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.Change,
                OrderbookData.IncrementalOrderbookUpdateType.Delete => IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.Delete,
                OrderbookData.IncrementalOrderbookUpdateType.New => IncrementalOrderbookUpdate.Types.IncrementalOrderbookUpdateType.New,
                _ => throw new InvalidOperationException($"Unknown UpdateType ({updateType})")
            };
        }
    }
}
