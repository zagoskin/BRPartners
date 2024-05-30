namespace BRP.Mailer.API.Application;

internal static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMediatR(config => config.RegisterServicesFromAssemblyContaining<Program>());
    }
}
