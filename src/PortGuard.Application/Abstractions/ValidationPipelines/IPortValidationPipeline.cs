using PortGuard.Application.Common.Results;

namespace PortGuard.Application.Abstractions.ValidationPipelines
{
    public interface IPortValidationPipeline
    {
        int Port { get; }
        int TimeOutMs { get; }
        Result Execute(ReadOnlySpan<byte> packet);
    }
}
