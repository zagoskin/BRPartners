using BRP.VendorManagement.Application.Common;
using BRP.VendorManagement.Application.Vendors;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BRP.VendorManagement.Infrastructure.Persistence.Vendors;
internal sealed class VendorRepository : 
    BaseRepository,
    IVendorRepository
{    
    public VendorRepository(VendorManagementDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
        
    }
    
    public async Task AddVendorAsync(Vendor vendor)
    {
        await _context.Vendors.AddAsync(vendor);
        await SaveChangesIfNotUnitOfWorkStartedAsync();
    }

    public Task<bool> ExistsByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default)
    {
        return _context.Vendors
            .AnyAsync(v => v.Id == vendorId, cancellationToken);
    }

    public async Task<Vendor?> GetVendorByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default)
    {
        return await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);
        
    }

    public async Task<List<Vendor>> ListVendorsAsync(CancellationToken cancellationToken = default)
    {
        return await _context
            .Vendors
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateVendorAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await SaveChangesIfNotUnitOfWorkStartedAsync();
    }
}
