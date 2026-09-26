using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Steps
{
    public class AllowedLengthsValidationStep : IPortValidationStep
    {
        private readonly HashSet<int> _allowedLengths;

        public AllowedLengthsValidationStep(int[] allowedLengths)
        {
            _allowedLengths = allowedLengths.ToHashSet();
        }
        public Result Validate(ReadOnlySpan<byte> packet)
        {
            return _allowedLengths.Contains(packet.Length)
                ? Result.Success()
                : Result.Fail($"Packet length {packet.Length} is not allowed.");
        }
    }
}
