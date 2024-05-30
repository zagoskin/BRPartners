namespace BRP.VendorManagement.Application.Common;

public interface IUnitOfWork
{
    void StartUnitOfWork();
    bool IsStarted { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
