using System.Text.Json.Serialization;

namespace BRPartners.Shared.Responses;
public class ListResponse<TResponse>
{
    [JsonPropertyName("items")]
    public List<TResponse> Items { get; init; } = new List<TResponse>();
}
