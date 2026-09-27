using PortGuard.Application.Common.Options;
using System.Net.Sockets;

namespace PortGuard.Application.Abstractions.Connections
{
    public interface IClientConnectionFactory
    {
        IClientConnection Create(Socket socket, PortPolicyOptions portPolicy);
    }
}
