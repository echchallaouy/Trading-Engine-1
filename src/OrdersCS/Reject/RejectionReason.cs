using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Rejects
{
    public enum RejectionReason
    {
        Unknown,
        OrderNotFound,
        OrderIdAlreadyPresent,
        OrderbookNotFound,
        InstrumentNotFound,
        AttemptingToModifyWrongSide,
        ModifyOrderDoesntModifyAnything,
    }
}
