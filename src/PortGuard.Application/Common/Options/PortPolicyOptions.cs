namespace PortGuard.Application.Common.Options
{
    public sealed record PortPolicyOptions
    {
        public required string Name { get; init; }
        public int Port { get; init; }
        public required string BackendInterface { get; init; }
        public int BackendPort { get; init; }
        public int TimeoutMs { get; init; } = 5000;
        public int? MinPayloadLength { get; init; }
        public int? MaxPayloadLength { get; init; }
        public int[]? PayloadAllowedLengths { get; init; }
        public string? HeaderPattern { get; init; }
    }
}
