using BRP.ContractManagement.Application.Contracts;
using BRP.VendorManagement.Application.Common;
using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BRP.VendorManagement.Infrastructure.Persistence.Contracts;
internal sealed class ContractRepository
    : BaseRepository
    , IContractRepository
{
    public ContractRepository(VendorManagementDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task AddContractAsync(Contract contract)
    {
        await _context.Contracts.AddAsync(contract);
        await SaveChangesIfNotUnitOfWorkStartedAsync();
    }

    public async Task<Contract?> GetContractByIdAsync(ContractId contractId, CancellationToken cancellationToken = default)
    {
        return await _context.Contracts
            .FirstOrDefaultAsync(c => c.Id == contractId, cancellationToken);
    }

    public async Task<List<Contract>> ListContractsAsync(CancellationToken cancellationToken = default)
    {
        return await _context
            .Contracts
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateContractAsync(Contract contract)
    {
        _context.Contracts.Update(contract);
        await SaveChangesIfNotUnitOfWorkStartedAsync();
    }
}
