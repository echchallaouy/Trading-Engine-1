using Eden.Proto.OrderEntry;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Fills;
using TradingEngineServer.Instrument;
using TradingEngineServer.Orders;
using TradingEngineServer.Orders.OrderStatuses;

namespace TradingEngineServer.OrderEntryCommunication
{
    internal class ProtoAdapter
    {
        public static OrderEntryModifyOrderStatus ModifyOrderStatusToProto(ModifyOrderStatus modifyOrderStatus)
        {
            return new OrderEntryModifyOrderStatus()
            {
                IsBuySide = modifyOrderStatus.IsBuySide,
                Price = modifyOrderStatus.Price,
                Quantity = modifyOrderStatus.Quantity,
                CreationTime = DateTimeToProto(modifyOrderStatus.CreationTime),
                OrderStatusCore = OrderStatusCoreToProto(modifyOrderStatus),
            };
        }

        public static OrderEntryNewOrderStatus NewOrderStatusToProto(NewOrderStatus newOrderStatus)
        {
            return new OrderEntryNewOrderStatus()
            {
                IsBuySide = newOrderStatus.IsBuySide,
                Price = newOrderStatus.Price,
                Quantity = newOrderStatus.Quantity,
                CreationTime = DateTimeToProto(newOrderStatus.CreationTime),
                OrderStatusCore = OrderStatusCoreToProto(newOrderStatus),
            };
        }

        public static OrderEntryCancelOrderStatus CancelOrderStatusToProto(CancelOrderStatus cancelOrderStatus)
        {
            return new OrderEntryCancelOrderStatus()
            {
                CreationTime = DateTimeToProto(cancelOrderStatus.CreationTime),
                OrderStatusCore = OrderStatusCoreToProto(cancelOrderStatus),
            };
        }

        private static OrderEntryOrderStatusCore OrderStatusCoreToProto(IOrderStatus orderStatus)
        {
            return new OrderEntryOrderStatusCore()
            {
                SecurityId = orderStatus.SecurityId,
                OrderId = orderStatus.OrderId,
                Username = orderStatus.Username,
            };
        }

        public static OrderEntryFill FillToProto(Fill fill)
        {
            return new OrderEntryFill()
            {
                AllocationAlgorithm = AllocationAlgorithmToProto(fill.AllocationAlgorithm),
                EventTime = DateTimeToProto(fill.EventTime),
                ExecutionId = fill.ExecutionId,
                FillExecutionId = fill.FillExecutionId,
                FillId = fill.FillId,
                FillQuantity = fill.FillQuantity,
                IsCompleteFill = fill.IsCompleteFill,
            };
        }

        public static OrderEntryRejection RejectionToProto(Rejects.Rejection rejection)
        {
            return new OrderEntryRejection()
            {
                OrderCore = OrderCoreToProto(rejection),
                RejectionReason = RejectionReasonToProto(rejection.RejectionReason),
            };
        }

        private static OrderEntryOrderCore OrderCoreToProto(IOrderCore orderCore)
        {
            return new OrderEntryOrderCore()
            {
                OrderId = orderCore.OrderId,
                SecurityId = orderCore.SecurityId,
                Username = orderCore.Username,
            };
        }

        private static OrderEntryRejection.Types.OrderEntryRejectionReason RejectionReasonToProto(Rejects.RejectionReason rejectionReason)
        {
            return rejectionReason switch
            {
                Rejects.RejectionReason.AttemptingToModifyWrongSide => OrderEntryRejection.Types.OrderEntryRejectionReason.AttemptingToModifyWrongSide,
                Rejects.RejectionReason.InstrumentNotFound => OrderEntryRejection.Types.OrderEntryRejectionReason.InstrumentNotFound,
                Rejects.RejectionReason.ModifyOrderDoesntModifyAnything => OrderEntryRejection.Types.OrderEntryRejectionReason.ModifyOrderDoesntModifyAnything,
                Rejects.RejectionReason.OrderbookNotFound => OrderEntryRejection.Types.OrderEntryRejectionReason.OrderbookNotFound,
                Rejects.RejectionReason.OrderIdAlreadyPresent => OrderEntryRejection.Types.OrderEntryRejectionReason.OrderIdAlreadyPresent,
                Rejects.RejectionReason.OrderNotFound => OrderEntryRejection.Types.OrderEntryRejectionReason.OrderNotFound,
                Rejects.RejectionReason.Unknown => OrderEntryRejection.Types.OrderEntryRejectionReason.Unknown,
                _ => throw new InvalidOperationException($"Unknown RejectionReason ({rejectionReason})"),
            };
        }

        private static OrderEntryFill.Types.OrderEntryFillAllocationAlgorithm AllocationAlgorithmToProto(AllocationAlgorithm allocationAlgorithm)
        {
            return allocationAlgorithm switch
            {
                AllocationAlgorithm.Fifo => OrderEntryFill.Types.OrderEntryFillAllocationAlgorithm.Fifo,
                AllocationAlgorithm.Lifo => OrderEntryFill.Types.OrderEntryFillAllocationAlgorithm.Lifo,
                AllocationAlgorithm.ProRata => OrderEntryFill.Types.OrderEntryFillAllocationAlgorithm.ProRata,
                AllocationAlgorithm.Unknown => OrderEntryFill.Types.OrderEntryFillAllocationAlgorithm.Unspecified,
                _ => throw new InvalidOperationException($"Unknown AllocationAlgorithm ({allocationAlgorithm})"),
            };
        }

        private static Timestamp DateTimeToProto(DateTime eventTime)
        {
            return Timestamp.FromDateTime(eventTime);
        }

        public static Order NewOrderFromProto(OrderEntryNewOrder orderEntryNewOrder)
        {
            return new Order(OrderEntryOrderCoreFromProto(orderEntryNewOrder.OrderCore), 
                orderEntryNewOrder.Price, 
                orderEntryNewOrder.Quantity, 
                orderEntryNewOrder.IsBuySide);
        }

        public static ModifyOrder ModifyOrderFromProto(OrderEntryModifyOrder orderEntryModifyOrder)
        {
            return new ModifyOrder(OrderEntryOrderCoreFromProto(orderEntryModifyOrder.OrderCore),
                orderEntryModifyOrder.Price,
                orderEntryModifyOrder.Quantity,
                orderEntryModifyOrder.IsBuySide);
        }

        public static CancelOrder CancelOrderFromProto(OrderEntryCancelOrder orderEntryCancelOrder)
        {
            return new CancelOrder(OrderEntryOrderCoreFromProto(orderEntryCancelOrder.OrderCore));
        }

        private static OrderCore OrderEntryOrderCoreFromProto(OrderEntryOrderCore orderEntryOrderCore)
        {
            return new OrderCore(orderEntryOrderCore.OrderId, orderEntryOrderCore.Username, orderEntryOrderCore.SecurityId);
        }
    }
}
