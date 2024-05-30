using BRP.VendorManagement.Application.Common;

namespace BRP.VendorManagement.Infrastructure.Persistence;
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly VendorManagementDbContext _context;

    public UnitOfWork(VendorManagementDbContext context)
    {
        _context = context;
    }

    private bool _isUnitOfWorkStarted = false;
    public void StartUnitOfWork()
    {
        _isUnitOfWorkStarted = true;
    }
    public bool IsStarted => _isUnitOfWorkStarted;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
