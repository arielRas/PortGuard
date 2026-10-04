using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.PortPipelines.Steps;

namespace PortGuard.Application.PortPipelines.Factories
{
    internal static class PacketLenghtStepFactory
    {
        public static IPortValidationStep? Create(PortPolicyOptions options)
        {
            if (!options.MinPayloadLength.HasValue && options.MaxPayloadLength.HasValue)
            {
                return new ExactLengthValidationStep(options.MaxPayloadLength.Value);
            }

            if (options.MinPayloadLength.HasValue && !options.MaxPayloadLength.HasValue)
            {
                return new ExactLengthValidationStep(options.MinPayloadLength.Value);
            }

            if (options.MinPayloadLength.HasValue && options.MaxPayloadLength.HasValue)
            {
                return options.MinPayloadLength.Value == options.MaxPayloadLength.Value
                    ? new ExactLengthValidationStep(options.MinPayloadLength.Value)
                    : new BetweenLengthValidationStep(options.MinPayloadLength.Value, options.MaxPayloadLength.Value);
            }

            if (options.PayloadAllowedLengths is not null && options.PayloadAllowedLengths.Length > 0)
            {
                return new AllowedLengthsValidationStep(options.PayloadAllowedLengths);
            }

            return null;
        }
    }
}
