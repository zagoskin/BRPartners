using BRP.VendorManagement.Application.Common;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;
using MediatR;

namespace BRP.VendorManagement.Application.Vendors.Queries.GetById;
internal sealed class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, ErrorOr<Vendor>>
{
    private readonly IVendorRepository _vendorRepository;

    public GetVendorByIdQueryHandler(IVendorRepository vendorRepository)
    {
        _vendorRepository = vendorRepository;        
    }

    public async Task<ErrorOr<Vendor>> Handle(GetVendorByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await _vendorRepository.GetVendorByIdAsync(
            VendorId.Create(request.VendorId), 
            cancellationToken);

        return vendor is null
            ? GeneralErrors.NotFound(request.VendorId, typeof(Vendor))
            : vendor;
    }
}
