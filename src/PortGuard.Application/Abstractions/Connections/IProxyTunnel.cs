namespace PortGuard.Application.Abstractions.Connections
{
    public interface IProxyTunnel
    {
        Task ForwardAsync(IClientConnection clientConnection, ReadOnlyMemory<byte> payload, CancellationToken token = default);
    }
}
