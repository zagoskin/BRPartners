using BRP.VendorManagement.Domain.VendorAggregate;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Vendors.Queries.List;
internal sealed class ListVendorsQueryHandler : IRequestHandler<ListVendorsQuery, ErrorOr<List<Vendor>>>
{
    private readonly IVendorRepository _vendorRepository;

    public ListVendorsQueryHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;
    }

    public async Task<ErrorOr<List<Vendor>>> Handle(ListVendorsQuery request, CancellationToken cancellationToken)
    {
        var vendors = await _vendorRepository.ListVendorsAsync(cancellationToken);        
        return vendors;
    }
}
