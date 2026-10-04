using PortGuard.Application.Abstractions.Connections;
using System.Net.Sockets;

namespace PortGuard.Infrastructure.Connections.Implementations
{
    public class DuplexProxyTunnel : IProxyTunnel
    {
        public async Task ForwardAsync(IClientConnection connection, ReadOnlyMemory<byte> payload, CancellationToken token = default)
        {
            using Socket backendSocket = new(SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true
            };

            await backendSocket.ConnectAsync(connection.TargetHost, connection.TargetPort, token);
            await backendSocket.SendAsync(payload, SocketFlags.None, token);

            using Stream clientStream = connection.GetStream();
            using NetworkStream backendStream = new(backendSocket, ownsSocket: false);

            Task clientToBackend = clientStream.CopyToAsync(backendStream, token);
            Task backendToClient = backendStream.CopyToAsync(clientStream, token);

            await Task.WhenAny(clientToBackend, backendToClient);
        }
    }
}
