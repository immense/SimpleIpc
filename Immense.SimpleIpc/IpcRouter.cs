using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SimpleIpc
{
    public interface IIpcRouter
    {

        /// <summary>
        /// Creates a message-based IIpcServer that handle messages via registered callbacks.
        /// Message callbacks can be registered using IIpcServer.On method.  The IpcServer
        /// will be assigned a randomly-generated ID for the pipe name.
        /// </summary>
        /// <returns>The newly-created IIpcServer.</returns>
        Task<IIpcServer> CreateServer();

        /// <summary>
        /// Creates a message-based IIpcServer that handle messages via registered callbacks.
        /// Message callbacks can be registered using IIpcServer.On method.
        /// </summary>
        /// <param name="pipeName">The pipe name to use for the IpcServer.</param>
        /// <returns></returns>
        Task<IIpcServer> CreateServer(string pipeName);

        bool TryGetServer(string pipeName, out IIpcServer server);

        bool TryRemoveServer(string pipeName, out IIpcServer server);

    }

    public class IpcRouter : IIpcRouter
    {
        private static IpcRouter? _default;
        public static IIpcRouter Default => _default ??= new IpcRouter(IpcConnectionFactory.Default, new LoggerFactory().CreateLogger<IpcRouter>());

        private static readonly ConcurrentDictionary<string, IIpcServer> _pipeStreams = new();

        private readonly IIpcConnectionFactory _serverFactory;
        private readonly ILogger<IpcRouter> _logger;

        public IpcRouter(IIpcConnectionFactory serverFactory, ILogger<IpcRouter> logger)
        {
            _serverFactory = serverFactory ?? throw new ArgumentNullException(nameof(serverFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<IIpcServer> CreateServer()
        {
            var pipeName = Guid.NewGuid().ToString();
            return await CreateServerInternal(pipeName);
        }

        public async Task<IIpcServer> CreateServer(string pipeName)
        {
            return await CreateServerInternal(pipeName);
        }

        public bool TryGetServer(string pipeName, out IIpcServer server)
        {
            return _pipeStreams.TryGetValue(pipeName, out server);
        }

        public bool TryRemoveServer(string pipeName, out IIpcServer server)
        {
            return _pipeStreams.TryRemove(pipeName, out server);
        }

        private async Task<IIpcServer> CreateServerInternal(string pipeName)
        {
            if (string.IsNullOrWhiteSpace(pipeName))
            {
                throw new ArgumentNullException(nameof(pipeName));
            }

            _logger.LogDebug("Creating pipe message server {name}.", pipeName);

            var serverConnection = await _serverFactory.CreateServer(pipeName);

            serverConnection.ReadingEnded += ServerConnection_ReadingEnded;

            if (!_pipeStreams.TryAdd(pipeName, serverConnection))
            {
                throw new ArgumentException("The pipe name is already in use.");
            }

            return serverConnection;
        }

        private void ServerConnection_ReadingEnded(object sender, IConnectionBase args)
        {
            if (_pipeStreams.TryRemove(args.PipeName, out var server))
            {
                server.ReadingEnded -= ServerConnection_ReadingEnded;
            }
            else
            {
                _logger.LogWarning("Pipe name {pipeName} not found.", args.PipeName);
            }
        }
    }
}
