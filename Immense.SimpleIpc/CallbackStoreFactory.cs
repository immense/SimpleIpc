using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleIpc
{
    public interface ICallbackStoreFactory
    {
        ICallbackStore Create();
    }

    public class CallbackStoreFactory : ICallbackStoreFactory
    {
        private readonly ILoggerFactory _loggerFactory;

        public CallbackStoreFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public ICallbackStore Create()
        {
            var logger = _loggerFactory.CreateLogger<CallbackStore>();
            return new CallbackStore(logger);
        }
    }
}
