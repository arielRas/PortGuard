namespace PortGuard.Application.Exceptions
{
    public sealed class BusinessRuleException : ApplicationException
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
