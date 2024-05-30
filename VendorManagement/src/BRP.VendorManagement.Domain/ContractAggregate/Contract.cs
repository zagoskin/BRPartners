using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.Common.Models;
using BRP.VendorManagement.Domain.ContractAggregate.Entities;
using BRP.VendorManagement.Domain.ContractAggregate.Events;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;

namespace BRP.VendorManagement.Domain.ContractAggregate;
public sealed class Contract : AggregateRoot<ContractId>
{
    public string Title { get; private set; } = null!;
    public VendorId VendorId { get; private set; } = null!;
    public DateTime DeadLineUtc { get; private set; }
    public decimal EstimatedValue { get; private set; }
    public ContractStatus Status { get; private set; }
    private readonly List<ContractHistory> _contractHistories = new();
    public IReadOnlyList<ContractHistory> ContractHistories => _contractHistories.AsReadOnly();
    internal Contract(
        IUser user,
        string title,
        VendorId vendorId,
        DateTime deadLineUtc,
        decimal estimatedValue,
        ContractStatus contractStatus = ContractStatus.PendingApproval,
        ContractId? id = null) : base(id ?? ContractId.CreateUnique())
    {        
        Title = title;
        VendorId = vendorId;
        DeadLineUtc = deadLineUtc;
        EstimatedValue = estimatedValue;
        Status = contractStatus;
        var userName = user.Name ?? "System";
        _contractHistories.Add(new ContractHistory(Status, userName, "Contract created"));
        RaiseDomainEvent(new ContractCreatedDomainEvent(
            Id.Value,
            userName));
    }

    public static ErrorOr<Contract> Create(
        IUser user,
        string title,
        VendorId vendorId,
        DateTime deadLineUtc,
        decimal estimatedValue,
        ContractStatus contractStatus = ContractStatus.PendingApproval,
        ContractId? id = null)
    {
        List<Error> errors = new();
        if (string.IsNullOrWhiteSpace(title))
        {
            errors.Add(ContractErrors.TitleIsRequired);
        }

        if (deadLineUtc < DateTime.UtcNow)
        {
            errors.Add(ContractErrors.DeadLineMustBeInTheFuture);
        }

        if (estimatedValue <= 0)
        {
            errors.Add(ContractErrors.EstimatedValueMustBeGreaterThanZero);
        }

        return errors.Count > 0
            ? errors
            : new Contract(user, title, vendorId, deadLineUtc, estimatedValue, contractStatus, id);
    }

    private Contract()
    {
    }

}
