using Microsoft.Extensions.Logging;
using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace SimpleIpc
{
    public interface IIpcClient : IConnectionBase
    {
        Task<bool> Connect(int timeout);
    }

    internal class IpcClient : ConnectionBase, IIpcClient
    {
        public IpcClient(
            string serverName,
            string pipeName,
            ICallbackStoreFactory callbackFactory, 
            ILogger<IpcClient> logger)
            : base(pipeName, callbackFactory, logger)
        {
            _pipeStream = new NamedPipeClientStream(
                serverName,
                pipeName,
                PipeDirection.InOut,
                PipeOptions.Asynchronous);
        }

        public async Task<bool> Connect(int timeout)
        {
            try
            {
                await _connectLock.WaitAsync();

                if (_pipeStream is NamedPipeClientStream clientPipe)
                {
                    await clientPipe.ConnectAsync(timeout);
                    _logger.LogDebug("Connection established for client pipe {id}.", PipeName);
                }
                else
                {
                    throw new InvalidOperationException("PipeStream is not of type NamedPipeClientStream.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to connect to IPC server.");
            }
            finally
            {
                _connectLock.Release();
            }

            return _pipeStream?.IsConnected == true;
        }

    }
}
