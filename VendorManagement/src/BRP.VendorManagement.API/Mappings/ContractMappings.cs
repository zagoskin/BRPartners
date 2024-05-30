using BRP.VendorManagement.Domain.ContractAggregate;
using BRPartners.Shared.Responses.VendorManagement;
using DomainContractStatus = BRP.VendorManagement.Domain.ContractAggregate.ContractStatus;
using ContractStatusResponse = BRPartners.Shared.Responses.VendorManagement.ContractStatus;
namespace BRP.VendorManagement.API.Mappings;

internal static class ContractMappings
{
    public static ContractListResponse ToResponse (this List<Contract> contracts)
    {
        return new ContractListResponse
        {
            Items = contracts.ConvertAll(c => c.ToResponse())
        };
    }

    public static ContractResponse ToResponse(this Contract contract)
    {
        return new ContractResponse
        {
            Id = contract.Id.Value.ToString(),
            VendorId = contract.VendorId.Value.ToString(),
            Title = contract.Title,
            DeadLineUtc = contract.DeadLineUtc,
            EstimatedValue = contract.EstimatedValue,
            Status = contract.Status.ToResponse()
        };
    }

    public static ContractStatusResponse ToResponse(this DomainContractStatus contractStatus)
    {
        return contractStatus switch
        {
            DomainContractStatus.PendingApproval => ContractStatusResponse.PendingApproval,
            DomainContractStatus.Approved => ContractStatusResponse.Approved,
            DomainContractStatus.Rejected => ContractStatusResponse.Rejected,
            DomainContractStatus.Cancelled => ContractStatusResponse.Cancelled,
            _ => throw new InvalidOperationException("Invalid contract status.")
        };
    }
}
