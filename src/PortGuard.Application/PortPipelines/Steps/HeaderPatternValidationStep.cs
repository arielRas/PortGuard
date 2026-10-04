using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;
using PortGuard.Application.Exceptions;

namespace PortGuard.Application.PortPipelines.Steps
{
    public class HeaderPatternValidationStep : IPortValidationStep
    {
        private readonly byte[] _expectedHeaderPattern;

        public HeaderPatternValidationStep(string expectedHeaderPattern)
        {
            _expectedHeaderPattern = GetExpectedHeaderPattern(expectedHeaderPattern);
        }

        public Result Validate(ReadOnlySpan<byte> packet)
        {
            if (!packet.StartsWith(_expectedHeaderPattern))
            {
                return Result.Fail($"Packet does not match the expected header pattern: {Convert.ToHexString(_expectedHeaderPattern)}.");
            }

            return Result.Success();
        }

        private static byte[] GetExpectedHeaderPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                throw new BusinessRuleException("Header pattern cannot be null or empty.");
            }

            string cleanHeader = pattern.Replace(" ", "").Trim();

            try
            {
                byte[] headerPattern = Convert.FromHexString(cleanHeader);

                return headerPattern;
            }
            catch (FormatException ex)
            {
                throw new HexFormatException(ex);
            }
        }
    }
}
