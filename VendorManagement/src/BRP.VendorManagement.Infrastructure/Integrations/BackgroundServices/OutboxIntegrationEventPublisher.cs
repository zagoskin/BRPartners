using System.Reflection;
using System.Text.Json;
using BRP.VendorManagement.Infrastructure.Persistence;
using BRPartners.Common.IntegrationEvents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace BRP.VendorManagement.Infrastructure.Integrations.BackgroundServices;
internal sealed class OutboxIntegrationEventPublisher : BackgroundService
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IIntegrationEventsPublisher _integrationEventsPublisher;
    private readonly ILogger<OutboxIntegrationEventPublisher> _logger;
    private readonly TimeSpan _period;
    public OutboxIntegrationEventPublisher(
        ILogger<OutboxIntegrationEventPublisher> logger,
        IServiceScopeFactory serviceScopeFactory,
        IIntegrationEventsPublisher integrationEventsPublisher, 
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _period = TimeSpan.FromSeconds(configuration.GetValue<int>("ServiceSecondIntervals:OutboxIntegrationEventPublisher"));
        _integrationEventsPublisher = integrationEventsPublisher;   
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(_period);
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await PublishIntegrationEventsFromDatabaseAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while publishing integration event");
            }
        }
    }

    private async Task PublishIntegrationEventsFromDatabaseAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VendorManagementDbContext>();

        var outboxIntegrationEvents = dbContext.OutboxIntegrationEvents.ToList();

        _logger.LogInformation("Read a total of {NumEvents} outbox integration events", outboxIntegrationEvents.Count);

        if (!outboxIntegrationEvents.Any())
        {
            _logger.LogInformation("No integration events found.");
            return;
        }

        foreach (var outboxIntegrationEvent in outboxIntegrationEvents)
        {
            var integrationEventsAssembly = Assembly.GetAssembly(typeof(IntegrationEvent));
            var type = integrationEventsAssembly?.GetTypes().FirstOrDefault(t => t.Name == outboxIntegrationEvent.Type);
            if (type is null)
            {
                _logger.LogWarning("Unknown Type {Type}", outboxIntegrationEvent.Type);
                return;
            }

            var integrationEvent = (IntegrationEvent)JsonSerializer.Deserialize(
                outboxIntegrationEvent.Content,
                type,
                _jsonSerializerOptions)!;            
            ArgumentNullException.ThrowIfNull(integrationEvent, "Integration event is null");

            _logger.LogInformation("Publishing event of type: {EventType}", integrationEvent.GetType().Name);
            _integrationEventsPublisher.PublishEvent(integrationEvent);
            _logger.LogInformation("Integration event published successfully");
        };

        dbContext.RemoveRange(outboxIntegrationEvents);
        await dbContext.SaveChangesAsync();
    }
}
