using PortGuard.Application.Common.Options;
using PortGuard.Application.Common.Results;
using System.Net;

namespace PortGuard.Application.PortPipelines.Validators
{
    internal static class PortPoliciesValidator
    {
        public static Result Validate(PortPolicyOptions options)
        {
            List<string> errors = [];

            if (options is null)
            {
                return Result.Fail("One on more policies cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(options.Name))
            {
                return Result.Fail("Name is required.");
            }

            if (options.Port < 1 || options.Port > 65535)
            {
                errors.Add($"Policy {options.Name} - Port must be between 1 and 65535.");
            }

            if (options.BackendPort < 1 || options.BackendPort > 65535)
            {
                errors.Add($"Policy {options.Name} - Backend port must be between 1 and 65535.");
            }

            if (string.IsNullOrWhiteSpace(options.BackendInterface))
            {
                errors.Add($"Policy {options.Name} - Backend interface is required.");

                return Result.Fail(string.Join(Environment.NewLine, errors));
            }

            if (!IPAddress.TryParse(options.BackendInterface, out _))
            {
                errors.Add($"Policy {options.Name} - Backend interface must be a valid IP address.");
                return Result.Fail(string.Join(Environment.NewLine, errors));
            }

            return errors.Count == 0
                ? Result.Success()
                : Result.Fail(string.Join(Environment.NewLine, errors));
        }
    }
}
