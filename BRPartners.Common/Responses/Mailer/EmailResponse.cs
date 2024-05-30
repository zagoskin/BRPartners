using System.Text.Json.Serialization;

namespace BRPartners.Shared.Responses.Mailer;

public sealed class EmailResponse
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("to")]
    public string To { get; init; } = null!;

    [JsonPropertyName("from")]
    public string From { get; init; } = null!;

    [JsonPropertyName("subject")]
    public string Subject { get; init; } = null!;

    [JsonPropertyName("body")]
    public string Body { get; init; } = null!;

    [JsonPropertyName("isHtmlBody")]
    public bool IsHtmlBody { get; init; } = false;

    [JsonPropertyName("dateTimeUtcProcessed")]
    public DateTime? DateTimeUtcProcessed { get; init; }
}
