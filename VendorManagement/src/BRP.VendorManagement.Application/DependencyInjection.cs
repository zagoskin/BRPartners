using BRP.VendorManagement.Application.Behaviors;
using BRP.VendorManagement.Application.Services;
using BRP.VendorManagement.Domain.Common.Abstractions;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BRP.VendorManagement.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddApplicationServices(configuration)
            .AddMediatRPipeline(configuration);
    }

    private static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSingleton<IUser, FakeUserService>();
    }

    private static IServiceCollection AddMediatRPipeline(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddValidatorsFromAssemblyContaining<IApplicationAssemblyMarker>(includeInternalTypes: true)
            .AddMediatR(options =>
            {
                options.RegisterServicesFromAssemblyContaining<IApplicationAssemblyMarker>();
                options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });
    }
}
