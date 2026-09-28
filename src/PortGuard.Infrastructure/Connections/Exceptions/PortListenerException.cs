namespace PortGuard.Infrastructure.Connections.Exceptions
{
    public sealed class PortListenerException : InfrastructureException
    {
        public PortListenerException(int port, Exception ex)
            : base($"Unknown error to listen on port {port}", ex)
        {
        }
    }
}
