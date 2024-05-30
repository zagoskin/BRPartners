using System.Text;
using System.Text.Json;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRPartners.Common.IntegrationEvents;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace BRP.VendorManagement.Infrastructure.Integrations;

internal sealed class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
    private readonly BrokerSettings _messageBrokerSettings;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web);

    public IntegrationEventsPublisher(IOptions<BrokerSettings> messageBrokerOptions)
    {
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
        _channel.ExchangeDeclare(_messageBrokerSettings.ExchangeName, ExchangeType.Fanout, durable: true);
    }

    public void PublishEvent(IntegrationEvent integrationEvent)
    {    
        var serializedIntegrationEvent = JsonSerializer.Serialize(
            integrationEvent,
            integrationEvent.GetType(),
            _jsonSerializerOptions);

        var body = Encoding.UTF8.GetBytes(serializedIntegrationEvent);

        _channel.BasicPublish(
            exchange: _messageBrokerSettings.ExchangeName,
            routingKey: string.Empty,
            body: body);
    }
}
