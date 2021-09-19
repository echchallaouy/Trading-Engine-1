using TradingEngineServer.Common;

namespace TradingEngineServer.Orders
{
    public class CancelOrder : IOrderCore
    {
        public CancelOrder(IOrderCore orderBase)
        {
            _orderBase = orderBase;
        }

        // PROPERTIES //
        public long OrderId => _orderBase.OrderId;
        public string Username => _orderBase.Username;
        public int SecurityId => _orderBase.SecurityId;

        // TOSTRING //
        public override string ToString()
        {
            return $"[{_orderBase}]";
        }

        private readonly IOrderCore _orderBase;
    }
}