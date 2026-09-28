using PortGuard.Application.DependencyInjection;
using PortGuard.Infrastructure;
using PortGuard.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices()
    .AddHostedService<PortGuardWorker>();

var host = builder.Build();

host.Run();
