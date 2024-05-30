using System.Text.Json.Serialization;

namespace BRPartners.Shared.Responses.VendorManagement;

public class VendorResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; init; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; init; } = null!;

    [JsonPropertyName("identifier")]
    public string Identifier { get; init; } = null!;
}
