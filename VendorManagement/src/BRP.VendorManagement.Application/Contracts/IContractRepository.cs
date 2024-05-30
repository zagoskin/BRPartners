using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;

namespace BRP.ContractManagement.Application.Contracts;
public interface IContractRepository
{
    Task<List<Contract>> ListContractsAsync(CancellationToken cancellationToken = default);
    Task<Contract?> GetContractByIdAsync(ContractId contractId, CancellationToken cancellationToken = default);
    Task AddContractAsync(Contract contract);
    Task UpdateContractAsync(Contract contract);
}
