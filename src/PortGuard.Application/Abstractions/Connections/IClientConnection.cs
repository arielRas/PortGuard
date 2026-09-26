namespace PortGuard.Application.Abstractions.Connections
{
    public interface IClientConnection
    {
        string Id { get; }
        int IncomingPort { get; }
        string RemoteIpAddress { get; }

        ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default);
    }
}
