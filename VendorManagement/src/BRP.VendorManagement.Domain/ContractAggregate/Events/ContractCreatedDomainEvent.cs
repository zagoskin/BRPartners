using BRP.VendorManagement.Domain.Common.Abstractions;

namespace BRP.VendorManagement.Domain.ContractAggregate.Events;
public record ContractCreatedDomainEvent(Guid ContractId, string DoneBy) : IDomainEvent;
