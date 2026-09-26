namespace PortGuard.Application.Abstractions.Connections
{
    public interface IConnectionDropper
    {
        ValueTask DropConnectionAsync(IClientConnection connection, string reason, CancellationToken token = default);
    }
}
