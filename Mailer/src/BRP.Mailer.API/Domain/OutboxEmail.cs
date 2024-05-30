using System.Text.Json.Serialization;

namespace BRP.Mailer.API.Domain;

internal sealed class OutboxEmail
{    
    public Guid Id { get; init; } = Guid.NewGuid();
    public string To { get; init; } = null!;
    public string From { get; init; } = null!;
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public bool IsHtmlBody { get; init; } = false;

    [JsonInclude]
    [JsonPropertyName("dateTimeUtcProcessed")]
    public DateTime? DateTimeUtcProcessed { get; private set; }

    [JsonConstructor]
    public OutboxEmail(string to, string from, string subject, string body, bool isHtmlBody, DateTime? dateTimeUtcProcessed = null)
    {
        To = to;
        From = from;
        Subject = subject;
        Body = body;
        IsHtmlBody = isHtmlBody;
    }

    
    private OutboxEmail() { }
}
