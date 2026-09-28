namespace PortGuard.Infrastructure.Connections.Exceptions
{
    public abstract class InfrastructureException : Exception
    {
        protected InfrastructureException(string message)
            : base(message)
        {
        }

        protected InfrastructureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
