using System.Text.Json.Serialization;

namespace BRPartners.Shared.Responses.VendorManagement;
public class ContractResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = null!;

    [JsonPropertyName("vendorId")]
    public string VendorId { get; init; } = null!;

    [JsonPropertyName("title")]
    public string Title { get; init; } = null!;

    [JsonPropertyName("description")]
    public DateTime DeadLineUtc { get; init; } 

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ContractStatus Status { get; init; } 

    [JsonPropertyName("estimatedValue")]
    public decimal EstimatedValue { get; init; }
}


public enum ContractStatus
{
    PendingApproval = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}
