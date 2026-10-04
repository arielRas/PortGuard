using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Steps
{
    public class ExactLengthValidationStep : IPortValidationStep
    {
        private readonly int _exactLength;

        public ExactLengthValidationStep(int exactLength)
        {
            _exactLength = exactLength;
        }

        public Result Validate(ReadOnlySpan<byte> packet)
        {
            return packet.Length == _exactLength
                ? Result.Success()
                : Result.Fail($"Packet length {packet.Length} is not the expected length of {_exactLength}.");
        }
    }
}
