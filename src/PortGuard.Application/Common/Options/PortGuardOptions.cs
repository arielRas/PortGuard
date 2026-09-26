namespace PortGuard.Application.Common.Options
{
    public sealed record PortGuardOptions
    {
        public required IEnumerable<PortPolicyOptions> PortPolicies { get; init; } = [];
    }
}
