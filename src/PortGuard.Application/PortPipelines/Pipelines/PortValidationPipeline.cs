using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines.Pipelines
{
    public class PortValidationPipeline : IPortValidationPipeline
    {
        private readonly int _port;
        private readonly IReadOnlyList<IPortValidationStep> _steps;
        public int Port => _port;

        public PortValidationPipeline(int port, IEnumerable<IPortValidationStep> steps)
        {
            _port = port;
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
