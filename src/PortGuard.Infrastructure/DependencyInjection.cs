using Microsoft.Extensions.DependencyInjection;
using PortGuard.Application.Abstractions.Connections;
using PortGuard.Infrastructure.Connections.Implementations;

namespace PortGuard.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IPortListener, PortListener>()
                .AddSingleton<IProxyTunnel, DuplexProxyTunnel>();
        }
    }
}
