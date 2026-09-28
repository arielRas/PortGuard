using PortGuard.Application.Common.Options;

namespace PortGuard.Application.Abstractions.Connections
{
    public interface IPortListener
    {
        Task StartListeningAsync(
            PortPolicyOptions options,
            Func<IClientConnection, CancellationToken, ValueTask> action,
            CancellationToken cancellationToken);
    }
}
