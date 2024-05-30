using BRP.VendorManagement.Domain.Common.Models;

namespace BRP.VendorManagement.Domain.ContractAggregate.Entities;
public sealed class ContractHistory : Entity<Guid>
{
    public ContractStatus Status { get; init; }
    public string Actor { get; init; } = null!;
    public string Message { get; init; } = null!;

    internal ContractHistory(ContractStatus status, string actor, string message, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Status = status;
        Actor = actor;
        Message = message;
    }

    private ContractHistory() : base()
    {
    }
}
