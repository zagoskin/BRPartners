using BRP.VendorManagement.Application.Contracts.Commands.Create;
using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;

namespace BRP.VendorManagement.Tests.Unit.Contracts;

public static class ContractFactory
{
    private const string ValidTitle = "Valid Title";
    private const string InValidTitle = "";
    private const decimal ValidEstimatedValue = 100_000;
    private const decimal InValidEstimatedValue = -100_000;
    private static readonly DateTime _validDeadLineUtc = DateTime.UtcNow.AddMonths(6);
    private static readonly DateTime _invalidDeadLineUtc = DateTime.UtcNow.AddMonths(-6);

    public static Contract CreateValidContract(IUser user, VendorId vendorId, ContractStatus contractStatus = ContractStatus.PendingApproval, ContractId? contractId = null)
    {
        return new Contract(
            user,
            ValidTitle,
            vendorId,
            _validDeadLineUtc,
            ValidEstimatedValue,
            contractStatus: contractStatus,
            id: contractId);
    }

    public static Contract CreateInValidContract(IUser user, VendorId vendorId)
    {
        return new Contract(
            user,
            InValidTitle,
            vendorId,
            _invalidDeadLineUtc,
            InValidEstimatedValue);
    }

    public static CreateContractCommand CreateFromContract(Contract contract)
    {
        return new CreateContractCommand(
            contract.VendorId.Value,
            contract.Title,
            contract.DeadLineUtc,
            contract.EstimatedValue);
    }
}
