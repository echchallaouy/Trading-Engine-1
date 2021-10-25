using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Logging
{
    public record LogInformation(LogLevel LogLevel, DateTime LogTime, int ThreadId, string ThreadName, string Message, string Module);
}
