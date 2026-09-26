using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Steps
{
    public class PacketLengthValidationStep : IPortValidationStep
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public PacketLengthValidationStep(int expectedLength)
        {
            _minLength = expectedLength;
            _maxLength = expectedLength;
        }

        public PacketLengthValidationStep(int minLength, int maxLength)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        public Result Validate(ReadOnlySpan<byte> packet)
        {

            if (packet.Length < _minLength || packet.Length > _maxLength)
            {
                return Result.Fail($"Packet length {packet.Length} is not within the expected range of {_minLength} to {_maxLength}.");
            }

            return Result.Success();
        }
    }
}
