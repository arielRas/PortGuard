using PortGuard.Application.Common.Results;

namespace PortGuard.Application.Abstractions.ValidationPipelines
{
    public interface IPortValidationStep
    {
        Result Validate(ReadOnlySpan<byte> packet);
    }
}
