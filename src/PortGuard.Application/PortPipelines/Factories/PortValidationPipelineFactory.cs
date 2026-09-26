using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.PortPipelines.Pipelines;
using PortGuard.Application.PortPipelines.Steps;

namespace PortGuard.Application.PortPipelines.Factories
{
    public class PortValidationPipelineFactory : IPortValidationPipelineFactory
    {
        public IPortValidationPipeline Create(PortPolicyOptions options)
        {
            List<IPortValidationStep> steps = [];

            IPortValidationStep? lenghtvalidationStep = PacketLenghtStepFactory.Create(options);

            if (lenghtvalidationStep != null)
            {
                steps.Add(lenghtvalidationStep);
            }

            if (options.HeaderPattern is not null)
            {
                steps.Add(new HeaderPatternValidationStep(options.HeaderPattern));
            }

            return new PortValidationPipeline(options.Port, options.TimeoutMs, steps);
        }
    }
}
