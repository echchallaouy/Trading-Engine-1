using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.Common
{
    /// <summary>
    /// Prototype.
    /// </summary>
    public interface IPrototype<T>
    {
        T Clone();
    }
}
