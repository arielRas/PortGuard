using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.Exceptions;
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

            return steps.Count > 0
                ? new PortValidationPipeline(options.Port, options.TimeoutMs, steps)
                : throw new BusinessRuleException($"No validation steps could be created for port {options.Port} and policy {options.Name}.");
        }
    }
}
