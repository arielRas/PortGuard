using PortGuard.Application.Abstractions.Connections;

namespace PortGuard.Application.Abstractions.ValidationPipelines
{
    public interface IPortGuardOrchestrator
    {
        ValueTask ProcessConnectionAsync(IClientConnection connection, CancellationToken token = default);
    }
}
