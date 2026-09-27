using PortGuard.Application.Abstractions.Connections;
using PortGuard.Application.Common.Options;
using System.Net.Sockets;

namespace PortGuard.Infrastructure.Connections.Implementations
{
    public class ClientConnectionFactory : IClientConnectionFactory
    {
        public IClientConnection Create(Socket socket, PortPolicyOptions portPolicy)
        {
            return new SocketClientConnection(socket, portPolicy.Port, portPolicy.BackendInterface, portPolicy.BackendPort);
        }
    }
}
