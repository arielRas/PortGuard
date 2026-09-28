using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PortGuard.Application.Abstractions.Connections;
using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.Common.Results;
using PortGuard.Application.Exceptions;
using PortGuard.Application.PortPipelines.Validators;
using System.Net.Sockets;

namespace PortGuard.Application.PortPipelines
{
    public class PortGuardOrchestrator : IPortGuardOrchestrator
    {
        private readonly IProxyTunnel _proxyTunnel;
        private readonly ILogger<PortGuardOrchestrator> _logger;
        private readonly Dictionary<int, IPortValidationPipeline> _portsPipelines;

        public PortGuardOrchestrator(
            IPortValidationPipelineFactory pipelineFactory,
            IProxyTunnel proxyTunnel,
            IOptions<PortGuardOptions> options,
            ILogger<PortGuardOrchestrator> logger)
        {
            _proxyTunnel = proxyTunnel;
            _logger = logger;
            _portsPipelines = CreatePipelines(pipelineFactory, options.Value);
        }
        public async ValueTask ProcessConnectionAsync(IClientConnection connection, CancellationToken token = default)
        {
            if (!_portsPipelines.TryGetValue(connection.IncomingPort, out IPortValidationPipeline? pipeline))
            {
                await connection.DropConnectionAsync(token);

                throw new BusinessRuleException($"No pipeline found for port {connection.IncomingPort}.");
            }

            byte[] buffer = new byte[1024];

            using CancellationTokenSource timeoutCt = CancellationTokenSource.CreateLinkedTokenSource(token);

            timeoutCt.CancelAfter(pipeline.TimeOutMs);

            int bytesRead;

            try
            {
                bytesRead = await connection.ReadAsync(buffer, timeoutCt.Token);
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested)
            {
                await HandleDropConnection(
                    connection,
                     $"Timeout while reading from connection {connection.Id} on port {connection.IncomingPort}.",
                    token);

                return;
            }
            catch (SocketException)
            {
                await HandleDropConnection(
                    connection,
                    $"Client connection ID: {connection.Id} was reset by the peer.",
                    token);

                return;
            }

            if (bytesRead == 0)
            {
                await HandleDropConnection(
                    connection,
                    $"Connection {connection.Id} on port {connection.IncomingPort} closed by client without sending data.",
                    token);

                return;
            }

            ReadOnlySpan<byte> packet = buffer.AsSpan(0, bytesRead);

            Result validationResult = pipeline.Execute(packet);

            if (validationResult.IsFailed)
            {
                await HandleDropConnection(
                    connection,
                    $"Connection {connection.Id} on port {connection.IncomingPort} failed validation: {validationResult.Error}.",
                    token);

                return;
            }

            await _proxyTunnel.ForwardAsync(connection, buffer.AsMemory(0, bytesRead), token);
        }

        private static Dictionary<int, IPortValidationPipeline> CreatePipelines(IPortValidationPipelineFactory pipelineFactory, PortGuardOptions options)
        {
            Dictionary<int, IPortValidationPipeline> portsPipelines = [];

            foreach (PortPolicyOptions policy in options.PortPolicies)
            {
                Result validationResult = PortPoliciesValidator.Validate(policy);

                if (validationResult.IsFailed)
                {
                    throw new BusinessRuleException(validationResult.Error);
                }

                IPortValidationPipeline pipeline = pipelineFactory.Create(policy);

                portsPipelines[policy.Port] = pipeline;
            }

            return portsPipelines;
        }

        private async ValueTask HandleDropConnection(IClientConnection connection, string message, CancellationToken token)
        {
            await connection.DropConnectionAsync(token);
            _logger.LogWarning("{Message} Dropped connection", message);
        }
    }
}
