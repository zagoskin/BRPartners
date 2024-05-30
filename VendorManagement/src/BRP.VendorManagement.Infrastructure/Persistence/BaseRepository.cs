using BRP.VendorManagement.Application.Common;

namespace BRP.VendorManagement.Infrastructure.Persistence;
internal abstract class BaseRepository
{
    protected readonly VendorManagementDbContext _context;
    protected readonly IUnitOfWork _unitOfWork;
    protected BaseRepository(VendorManagementDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    protected async Task SaveChangesIfNotUnitOfWorkStartedAsync(CancellationToken token = default)
    {
        if (_unitOfWork.IsStarted)
        {
            return;
        }
        await _unitOfWork.SaveChangesAsync(token);
    }
}
