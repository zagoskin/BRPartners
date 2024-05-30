using BRP.Mailer.API.Application.Integrations;
using BRP.Mailer.API.Domain;
using BRP.Mailer.API.Infrastructure.BackgroundServices;
using BRP.Mailer.API.Infrastructure.Services;
using MongoDB.Driver;

namespace BRP.Mailer.API.Infrastructure;

internal static class DependencyInjection
{
    internal static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddSettings(configuration)
            .AddMongoDB(configuration)
            .AddHostedService<EmailSendingBackgroundService>()
            .AddHostedService<ConsumeIntegrationEventsPublisher>()
            .AddTransient<IOutboxEmailSendingService, DefaultOutboxEmailSendingService>()
            .AddTransient<IOutboxService, MongoDbOutboxService>();
        
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<MongoDBSettings>(configuration.GetSection(MongoDBSettings.SectionName))
            .Configure<MessageBrokerSettings>(configuration.GetSection(MessageBrokerSettings.SectionName));
    }

    private static IServiceCollection AddMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(MongoDBSettings.SectionName).Get<MongoDBSettings>()
            ?? throw new InvalidOperationException($"Configuration section '{MongoDBSettings.SectionName}' is missing");

        return services
            .AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(settings.ConnectionString);
            })
            .AddSingleton(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client!.GetDatabase(settings!.DatabaseName);
            })
            .AddTransient(sp =>
            {
                var database = sp.GetRequiredService<IMongoDatabase>();
                return database!.GetCollection<OutboxEmail>("OutboxEmailCollection");
            });
        
    }
}
