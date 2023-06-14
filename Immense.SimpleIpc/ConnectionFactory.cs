using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SimpleIpc
{
    public interface IIpcConnectionFactory
    {
        Task<IIpcClient> CreateClient(string serverName, string pipeName);
        Task<IIpcServer> CreateServer(string pipeName);
    }

    public class IpcConnectionFactory : IIpcConnectionFactory
    {
        private static IIpcConnectionFactory? _default;
        private readonly ICallbackStoreFactory _callbackFactory;
        private readonly ILoggerFactory _loggerFactory;

        public IpcConnectionFactory(ICallbackStoreFactory callbackFactory, ILoggerFactory loggerFactory)
        {
            _callbackFactory = callbackFactory;
            _loggerFactory = loggerFactory;
        }

        public static IIpcConnectionFactory Default =>
            _default ??= 
            new IpcConnectionFactory(new CallbackStoreFactory(new LoggerFactory()), new LoggerFactory());

        public Task<IIpcClient> CreateClient(string serverName, string pipeName)
        {
            var client = new IpcClient(
                serverName, 
                pipeName, 
                _callbackFactory, 
                _loggerFactory.CreateLogger<IpcClient>());
            return Task.FromResult((IIpcClient)client);
        }

        public Task<IIpcServer> CreateServer(string pipeName)
        {
            var server = new IpcServer(
                pipeName, 
                _callbackFactory, 
                _loggerFactory.CreateLogger<IpcServer>());
            return Task.FromResult((IIpcServer)server);
        }
    }
}
