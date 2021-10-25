using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Orders
{
    public interface IOrderCore
    {
        public long OrderId { get; }
        public string Username { get; }
        public int SecurityId { get; }
    }

    public class OrderCore : IOrderCore
    {
        public OrderCore(long orderId, string username, int securityId)
        {
            OrderId = orderId;
            Username = username;
            SecurityId = securityId;
        }

        public long OrderId { get; private set; }
        public string Username { get; private set; }
        public int SecurityId { get; private set; }
        public override string ToString()
        {
            return string.Format("[OrderId={0}] - Username={1} Security={2}", OrderId, SecurityId, Username);
        }
    }
}
