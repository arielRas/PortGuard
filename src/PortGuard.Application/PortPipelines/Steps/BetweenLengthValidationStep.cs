using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Steps
{
    public class BetweenLengthValidationStep : IPortValidationStep
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public BetweenLengthValidationStep(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }
        public Result Validate(ReadOnlySpan<byte> packet)
        {
            return packet.Length >= _minLength && packet.Length <= _maxLength
                ? Result.Success()
                : Result.Fail($"Packet length {packet.Length} is not within the expected range of {_minLength} to {_maxLength}.");
        }
    }
}
