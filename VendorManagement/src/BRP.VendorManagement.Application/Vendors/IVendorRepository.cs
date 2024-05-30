using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;

namespace BRP.VendorManagement.Application.Vendors;
public interface IVendorRepository
{
    Task<List<Vendor>> ListVendorsAsync(CancellationToken cancellationToken = default);
    Task<Vendor?> GetVendorByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default);
    Task AddVendorAsync(Vendor vendor);
    Task UpdateVendorAsync(Vendor vendor);
    Task<bool> ExistsByIdAsync(VendorId vendorId, CancellationToken cancellationToken = default);
}
