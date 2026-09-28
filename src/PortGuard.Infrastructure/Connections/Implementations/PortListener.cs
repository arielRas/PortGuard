using Microsoft.Extensions.Logging;
using PortGuard.Application.Abstractions.Connections;
using PortGuard.Application.Common.Options;
using PortGuard.Infrastructure.Connections.Exceptions;
using System.Net;
using System.Net.Sockets;

namespace PortGuard.Infrastructure.Connections.Implementations
{
    public class PortListener : IPortListener
    {
        private readonly ILogger<PortListener> _logger;

        public PortListener(ILogger<PortListener> logger)
        {
            _logger = logger;
        }

        public async Task StartListeningAsync(
            PortPolicyOptions options,
            Func<IClientConnection, CancellationToken, ValueTask> action,
            CancellationToken cancellationToken)
        {
            TcpListener listener = new(IPAddress.Any, options.Port);

            try
            {
                listener.Start();

                while (!cancellationToken.IsCancellationRequested)
                {
                    Socket socket;

                    try
                    {
                        socket = await listener.AcceptSocketAsync(cancellationToken);
                    }
                    catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
                    {
                        continue;
                    }

                    _ = ProcessIncomingSocketAsync(socket, options, action, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new PortListenerException(options.Port, ex);
            }
            finally
            {
                listener.Stop();

                _logger.LogWarning("Stoping listener on port {Port}", options.Port);
            }
        }

        private async Task ProcessIncomingSocketAsync(
            Socket socket,
            PortPolicyOptions policy,
            Func<IClientConnection, CancellationToken, ValueTask> action,
            CancellationToken token)
        {
            try
            {
                SocketClientConnection connection = new(socket, policy.Port, policy.BackendInterface, policy.BackendPort);
                await action(connection, token);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unknown error processing connection on port {Port}", policy.Port);
            }
        }
    }
}
