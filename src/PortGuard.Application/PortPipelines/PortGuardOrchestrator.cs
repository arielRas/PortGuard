using Microsoft.Extensions.Options;
using PortGuard.Application.Abstractions.Connections;
using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.Common.Results;

namespace PortGuard.Application.PortPipelines
{
    public class PortGuardOrchestrator : IPortGuardOrchestrator
    {
        private readonly IConnectionDropper _connectionDropper;
        private readonly IProxyTunnel _proxyTunnel;
        private readonly Dictionary<int, IPortValidationPipeline> _portsPipelines;

        public PortGuardOrchestrator(
            IOptions<PortGuardOptions> options,
            IPortValidationPipelineFactory pipelineFactory,
            IConnectionDropper connectionDropper,
            IProxyTunnel proxyTunnel)
        {
            _connectionDropper = connectionDropper;
            _proxyTunnel = proxyTunnel;
            _portsPipelines = CreatePipelines(pipelineFactory, options.Value);
        }
        public async ValueTask ProcessConnectionAsync(IClientConnection connection, CancellationToken token = default)
        {
            if (!_portsPipelines.TryGetValue(connection.IncomingPort, out IPortValidationPipeline? pipeline))
            {
                string message = $"No pipeline found for port {connection.IncomingPort}. Dropping connection.";

                await _connectionDropper.DropConnectionAsync(connection, message, token);

                return;
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
                string message = $"Timeout while reading from connection {connection.Id} on port {connection.IncomingPort}. Dropping connection.";
                await _connectionDropper.DropConnectionAsync(connection, message, token);
                return;
            }

            if (bytesRead == 0)
            {
                string message = $"Connection {connection.Id} on port {connection.IncomingPort} closed by client without sending data. Dropping connection.";
                await _connectionDropper.DropConnectionAsync(connection, message, token);
                return;
            }

            ReadOnlySpan<byte> packet = buffer.AsSpan(0, bytesRead);

            Result validationResult = pipeline.Execute(packet);

            if (validationResult.IsFailed)
            {
                string message = $"Connection {connection.Id} on port {connection.IncomingPort} failed validation: {validationResult.Error}. Dropping connection.";
                await _connectionDropper.DropConnectionAsync(connection, message, token);
                return;
            }

            await _proxyTunnel.ForwardAsync(connection, buffer.AsMemory(0, bytesRead), token);
        }

        private static Dictionary<int, IPortValidationPipeline> CreatePipelines(IPortValidationPipelineFactory pipelineFactory, PortGuardOptions options)
        {
            Dictionary<int, IPortValidationPipeline> portsPipelines = [];

            foreach (PortPolicyOptions policy in options.PortPolicies)
            {
                IPortValidationPipeline pipeline = pipelineFactory.Create(policy);

                portsPipelines[policy.Port] = pipeline;
            }

            return portsPipelines;
        }
    }
}
