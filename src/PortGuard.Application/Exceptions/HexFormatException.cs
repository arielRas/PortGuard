namespace PortGuard.Application.Exceptions
{
    public sealed class HexFormatException : Exception
    {
        public HexFormatException() : base("Invalid hexadecimal format. Characters must be between 0-9 or A-F.")
        {
        }

        public HexFormatException(Exception innerException) :
            base("Invalid hexadecimal format. Characters must be between 0-9 or A-F.", innerException)
        {
        }
    }
}
