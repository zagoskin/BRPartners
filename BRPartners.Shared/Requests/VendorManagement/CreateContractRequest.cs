using System.Text.Json.Serialization;

namespace BRPartners.Shared.Requests.VendorManagement;
public class CreateContractRequest
{
    [JsonPropertyName("vendorId")]
    public Guid VendorId { get; init; } 

    [JsonPropertyName("title")]
    public string Title { get; init; } = null!;    

    [JsonPropertyName("deadLineUtc")]
    public DateOnly DeadLineUtc { get; init; }

    [JsonPropertyName("estimatedValue")]
    public decimal EstimatedValue { get; init; }
}
