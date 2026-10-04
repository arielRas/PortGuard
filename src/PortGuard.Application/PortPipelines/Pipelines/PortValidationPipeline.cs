using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Pipelines
{
    public class PortValidationPipeline : IPortValidationPipeline
    {
        private readonly IReadOnlyList<IPortValidationStep> _steps;
        public int Port { get; }
        public int TimeOutMs { get; }

        public PortValidationPipeline(int port, int timeOutMs, IEnumerable<IPortValidationStep> steps)
        {
            Port = port;
            TimeOutMs = timeOutMs;
            _steps = steps.ToList();
        }

        public Result Execute(ReadOnlySpan<byte> packet)
        {
            foreach (IPortValidationStep step in _steps)
            {
                Result result = step.Validate(packet);

                if (result.IsFailed)
                {
                    return result;
                }
            }

            return Result.Success();
        }
    }
}
