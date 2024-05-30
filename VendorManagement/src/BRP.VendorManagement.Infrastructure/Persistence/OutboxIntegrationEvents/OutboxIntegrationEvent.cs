namespace BRP.VendorManagement.Infrastructure.Persistence.OutboxIntegrationEvents;

public sealed class OutboxIntegrationEvent
{
    public long Id { get; }
    public string Type { get; } = null!;
    public string Content { get; } = null!;
    public DateTime CreatedOnUtc { get; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public OutboxIntegrationEvent(string type, string content)
    {
        Type = type;
        Content = content;
        CreatedOnUtc = DateTime.UtcNow;
    }

    public void MarkAsProcessed(string? error)
    {
        ProcessedOnUtc = DateTime.UtcNow;
        Error = error;
    }
    private OutboxIntegrationEvent() { }
}
