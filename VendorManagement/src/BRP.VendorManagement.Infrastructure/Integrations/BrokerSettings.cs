namespace BRP.VendorManagement.Infrastructure.Integrations;

internal class BrokerSettings
{
    public const string SectionName = "BrokerSettings";
    public string HostName { get; init; } = null!;
    public int Port { get; init; }
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string QueueName { get; init; } = null!;
    public string ExchangeName { get; init; } = null!;
}
