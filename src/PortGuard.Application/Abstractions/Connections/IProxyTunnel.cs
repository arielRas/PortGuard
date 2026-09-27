namespace PortGuard.Application.Abstractions.Connections
{
    public interface IProxyTunnel
    {
        Task ForwardAsync(IClientConnection connection, ReadOnlyMemory<byte> payload, CancellationToken token = default);
    }
}
