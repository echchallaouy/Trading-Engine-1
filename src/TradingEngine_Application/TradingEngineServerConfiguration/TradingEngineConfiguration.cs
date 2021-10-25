using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Core.Configuration
{
    public class TradingEngineServerConfiguration
    {
        public OrderEntryServer OrderEntryServer { get; set; }
    }

    public class OrderEntryServer
    {
        public int Port { get; set; }
    }
}
