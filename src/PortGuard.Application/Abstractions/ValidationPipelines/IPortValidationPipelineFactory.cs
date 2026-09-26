using PortGuard.Application.Common.Options;

namespace PortGuard.Application.Abstractions.ValidationPipelines
{
    public interface IPortValidationPipelineFactory
    {
        IPortValidationPipeline Create(PortPolicyOptions options);
    }
}
