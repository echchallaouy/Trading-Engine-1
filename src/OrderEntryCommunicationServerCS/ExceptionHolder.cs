using System;
using System.Collections.Generic;
using System.Text;

namespace TradingEngineServer.OrderEntryCommunicationServer
{
    internal interface IExceptionHolder
    {
        bool ContainsException { get; }
        Exception Exception { get; }
    }

    internal class ExceptionHolder : IExceptionHolder
    {

        // CONSTRUCTORS //
        public ExceptionHolder()
        {
            _exception = null;
        }

        public ExceptionHolder(Exception exception)
        {
            _exception = exception;
        }

        // PROPERTIES //
        public bool ContainsException
        {
            get
            {
                return _exception != null;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        private readonly Exception _exception;
    }
}
