using Microsoft.Extensions.Options;
using PortGuard.Application.Abstractions.Connections;
using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.Exceptions;
using PortGuard.Infrastructure.Connections.Exceptions;

namespace PortGuard.Workers
{
    public class PortGuardWorker : BackgroundService
    {
        private readonly ILogger<PortGuardWorker> _logger;
        private readonly IPortListener _portListener;
        private readonly IPortGuardOrchestrator _orchestrator;
        private readonly PortGuardOptions _options;

        public PortGuardWorker(
            IPortGuardOrchestrator orchestrator,
            IPortListener portListener,
            IOptions<PortGuardOptions> options,
            ILogger<PortGuardWorker> logger)
        {
            _orchestrator = orchestrator;
            _portListener = portListener;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> listenerTasks = [];

            try
            {
                foreach (PortPolicyOptions policy in _options.PortPolicies)
                {
                    Task listenerTask = _portListener.StartListeningAsync(policy, _orchestrator.ProcessConnectionAsync, stoppingToken);

                    listenerTasks.Add(listenerTask);
                }

                string portList = string.Join(", ", _options.PortPolicies.Select(p => p.Port));

                _logger.LogWarning("The application is listening on ports: {PortList}", portList);

                await Task.WhenAll(listenerTasks);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogWarning("The application is shutting down.");
            }
            catch (InfrastructureException ex)
            {
                _logger.LogCritical(ex, "{message}", ex.Message);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogError("{message}", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred: {message}", ex.Message);
            }
        }
    }
}
