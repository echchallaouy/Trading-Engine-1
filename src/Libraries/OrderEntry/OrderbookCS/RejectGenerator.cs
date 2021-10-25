using System;
using System.Collections.Generic;
using System.Text;
using TradingEngineServer.Orders;
using TradingEngineServer.Rejects;

namespace TradingEngineServer.Orderbook
{
    public sealed class RejectGenerator
    {
        public static bool TryRejectCancelOrder(CancelOrder cancelOrder, IReadOnlyOrderbook orderbook, out Rejection rejection)
        {
            if (!orderbook.ContainsOrder(cancelOrder.OrderId))
            {
                rejection = RejectionCreator.GenerateOrderCoreReject(cancelOrder, RejectionReason.OrderNotFound);
                return true;
            }
            rejection = null;
            return false;
        }

        public static bool TryRejectModifyOrder(ModifyOrder modifyOrder, IReadOnlyOrderbook orderbook, out Rejection rejection)
        {
            var modifyOrderType = orderbook.GetModifyOrderType(modifyOrder);
            if (modifyOrderType == ModifyOrderType.NoChange)
            {
                rejection = RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.ModifyOrderDoesntModifyAnything);
                return true;
            }
            else if (modifyOrderType == ModifyOrderType.Unknown)
            {
                rejection = RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.OrderNotFound);
                return true;
            }
            else if (!orderbook.ContainsOrder(modifyOrder.OrderId))
            {
                rejection = RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.OrderNotFound);
                return true;
            }
            else if (orderbook.TryGetOrder(modifyOrder.OrderId, out var order))
            {
                if (order.IsBuySide != modifyOrder.IsBuySide)
                {
                    rejection = RejectionCreator.GenerateOrderCoreReject(modifyOrder, RejectionReason.AttemptingToModifyWrongSide);
                    return true;
                }
            }

            rejection = null;
            return false;
        }

        public static bool TryRejectNewOrder(Order order, IReadOnlyOrderbook orderbook, out Rejection rejection)
        {
            if (orderbook.ContainsOrder(order.OrderId))
            {
                rejection = RejectionCreator.GenerateOrderCoreReject(order, RejectionReason.OrderIdAlreadyPresent);
                return true;
            }
            rejection = null;
            return false;
        }
    }
}
