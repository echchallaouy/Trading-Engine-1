using System;
using System.Collections.Generic;
using System.Text;
using TradingClient.Core;

namespace ConsoleTradingApplication.Core
{
    public static class CommandLineConverter
    {
        public static NewOrderRequest CreateNewOrderRequest(string input)
        {
            // Fill out later.
            return new NewOrderRequest();
        }

        public static CancelOrderRequest CreateCancelOrderRequest(string input)
        {
            // Fill out later.
            return new CancelOrderRequest();
        }

        public static ModifyOrderRequest CreateModifyOrderRequest(string input)
        {
            // Fill out later.
            return new ModifyOrderRequest();
        }
    }
}
