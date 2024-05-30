using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Common;
using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Infrastructure.Integrations;
using BRP.VendorManagement.Infrastructure.Integrations.BackgroundServices;
using BRP.VendorManagement.Infrastructure.Persistence;
using BRP.VendorManagement.Infrastructure.Persistence.Contracts;
using BRP.VendorManagement.Infrastructure.Persistence.Interceptors;
using BRP.VendorManagement.Infrastructure.Persistence.Vendors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BRP.VendorManagement.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSettings(configuration)
            .AddMediatRHandlers(configuration)
            .AddBackgroundServices(configuration)
            .AddInterceptors(configuration)
            .AddEFInMemoryContext(configuration)
            .AddRepositories(configuration);
    }

    private static IServiceCollection AddBackgroundServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IIntegrationEventsPublisher, IntegrationEventsPublisher>()
            .AddHostedService<OutboxIntegrationEventPublisher>()
            .AddHostedService<OutboxDomainEventPublisher>();
    }

    private static IServiceCollection AddMediatRHandlers(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMediatR(options => options.RegisterServicesFromAssemblyContaining<IInfrastructureAssemblyMarker>());
    }

    private static IServiceCollection AddInterceptors(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<ISaveChangesInterceptor, OutboxDomainEventWriterInterceptor>()
            .AddScoped<ISaveChangesInterceptor, CreatedAndModifiedDateRecorderInterceptor>();
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<BrokerSettings>(configuration.GetSection(BrokerSettings.SectionName));
    }

    private static IServiceCollection AddEFInMemoryContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbContext<VendorManagementDbContext>((serviceProvider, options) =>
            {
                options.AddInterceptors(serviceProvider.GetServices<ISaveChangesInterceptor>());
                // migrations were simulated with an SQL server provider
                //options.UseSqlServer(configuration.GetConnectionString("VendorManagement")!);
                options.UseInMemoryDatabase("VendorManagement");
            });
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IContractRepository, ContractRepository>()
            .AddScoped<IVendorRepository, VendorRepository>();
    }
}
