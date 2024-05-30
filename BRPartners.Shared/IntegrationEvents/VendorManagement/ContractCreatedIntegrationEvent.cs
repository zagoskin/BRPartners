using BRPartners.Shared.IntegrationEvents;

namespace BRPartners.Shared.IntegrationEvents.VendorManagement;
public class ContractCreatedIntegrationEvent : IntegrationEvent
{
    public string ContractId { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string VendorId { get; set; } = null!;
    public DateTime DeadLineUtc { get; set; }
    public decimal EstimatedValue { get; set; }
    public string DoneBy { get; set; } = null!;
    public override string Type { get; set; } = nameof(ContractCreatedIntegrationEvent);
}
