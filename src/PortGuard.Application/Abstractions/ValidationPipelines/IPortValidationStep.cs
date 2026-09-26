using PortGuard.Application.Common.Results;

namespace PortGuard.Application.Abstractions.ValidationPipelines
{
    public interface IPortValidationStep
    {
        string Name { get; }
        Result Validate(ReadOnlySpan<byte> packet);
    }
}
