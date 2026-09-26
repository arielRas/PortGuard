using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortGuard.Application.Abstractions.ValidationPipelines;
using PortGuard.Application.Common.Options;
using PortGuard.Application.PortPipelines;
using PortGuard.Application.PortPipelines.Factories;

namespace PortGuard.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddOptions(configuration)
                .AddServices();
        }

        private static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PortGuardOptions>(configuration.GetSection(nameof(PortGuardOptions)));
            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddSingleton<IPortValidationPipelineFactory, PortValidationPipelineFactory>()
                .AddSingleton<IPortGuardOrchestrator, PortGuardOrchestrator>();
        }
    }
}