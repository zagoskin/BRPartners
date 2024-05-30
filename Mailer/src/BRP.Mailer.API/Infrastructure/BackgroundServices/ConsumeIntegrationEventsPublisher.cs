using System.Reflection;
using System.Text;
using System.Text.Json;
using BRPartners.Shared.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BRP.Mailer.API.Infrastructure.BackgroundServices;


internal sealed class EmailSendingBackgroundService : BackgroundService
{
    private readonly ILogger<EmailSendingBackgroundService> _logger;
    private readonly IOutboxEmailSendingService _emailSendingService;
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    public EmailSendingBackgroundService(ILogger<EmailSendingBackgroundService> logger, IOutboxEmailSendingService emailSendingService)
    {
        _logger = logger;
        _emailSendingService = emailSendingService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[{Handler}] starting...",
            nameof(EmailSendingBackgroundService));

        using PeriodicTimer timer = new(_period);
        while (!stoppingToken.IsCancellationRequested &&
            await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("[{Handler}] Check for and send emails...",
                    nameof(EmailSendingBackgroundService));
                await _emailSendingService.CheckForAndSendEmailsAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception occurred while processing outbox");
            }
        }
        _logger.LogInformation("[{Handler}] stopping...",
            nameof(EmailSendingBackgroundService));
    }
}

internal sealed class ConsumeIntegrationEventsPublisher : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ConsumeIntegrationEventsPublisher> _logger;
    private readonly CancellationTokenSource _cts;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly MessageBrokerSettings _messageBrokerSettings;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public ConsumeIntegrationEventsPublisher(
        ILogger<ConsumeIntegrationEventsPublisher> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<MessageBrokerSettings> messageBrokerOptions)
    {
        _logger = logger;
        _cts = new CancellationTokenSource();
        _serviceScopeFactory = serviceScopeFactory;

        _messageBrokerSettings = messageBrokerOptions.Value;

        IConnectionFactory connectionFactory = new ConnectionFactory
        {
            HostName = _messageBrokerSettings.HostName,
            Port = _messageBrokerSettings.Port,
            UserName = _messageBrokerSettings.UserName,
            Password = _messageBrokerSettings.Password
        };

        _connection = connectionFactory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(
            _messageBrokerSettings.ExchangeName,
            ExchangeType.Fanout,
            durable: true);

        _channel.QueueDeclare(
            queue: _messageBrokerSettings.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            _messageBrokerSettings.QueueName,
            _messageBrokerSettings.ExchangeName,
            routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += PublishIntegrationEventAsync;

        _channel.BasicConsume(_messageBrokerSettings.QueueName, autoAck: false, consumer);
    }

    private async void PublishIntegrationEventAsync(object? sender, BasicDeliverEventArgs eventArgs)
    {
        if (_cts.IsCancellationRequested)
        {
            _logger.LogInformation("Cancellation requested, not consuming integration event.");
            return;
        }

        try
        {
            _logger.LogInformation("Received integration event. Reading message from queue.");

            using var scope = _serviceScopeFactory.CreateScope();

            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var parsedEvent = DeserializeEvent(message);

            _logger.LogInformation(
                "Received integration event of type: {IntegrationEventType}. Publishing event.",
                parsedEvent.Type);            

            var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
            await publisher.Publish(parsedEvent);

            _logger.LogInformation("Integration event published in Gym Management service successfully. Sending ack to message broker.");

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred while consuming integration event");
            // Nack the message logic, usar alguma dead letter ou algo do tipo
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting integration event consumer background service.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cts.Cancel();
        _cts.Dispose();
        return Task.CompletedTask;
    }

    private IntegrationEvent DeserializeEvent(string message)
    {
        var integrationEvent = JsonSerializer.Deserialize<IntegrationEvent>(message, _jsonSerializerOptions);
        ArgumentNullException.ThrowIfNull(integrationEvent, nameof(integrationEvent));

        var integrationEventsAssembly = Assembly.GetAssembly(typeof(IntegrationEvent));
        var type = integrationEventsAssembly?.GetTypes().FirstOrDefault(t => t.Name == integrationEvent.Type);
        if (type is null)
        {
            _logger.LogWarning("Unknown Type {Type}", integrationEvent.Type);
            throw new InvalidOperationException($"Unknown Type {integrationEvent.Type}");
        }

        var parsedEvent = (IntegrationEvent)JsonSerializer.Deserialize(
            message,
            type,
            _jsonSerializerOptions)!;
        ArgumentNullException.ThrowIfNull(parsedEvent, "Integration event is null");

        return parsedEvent;
    }
}
