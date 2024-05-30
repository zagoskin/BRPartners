using BRP.VendorManagement.Domain.VendorAggregate;
using BRPartners.Shared.Responses.VendorManagement;

namespace BRP.VendorManagement.API.Mappings;

internal static class VendorMappings
{
    public static VendorResponse ToResponse(this Vendor vendor)
    {
        return new VendorResponse
        {
            Id = vendor.Id.Value.ToString(),
            Name = vendor.Name,
            Email = vendor.Email.Value,
            Identifier = vendor.Identifier
        };
    }

    public static VendorListResponse ToResponse(this List<Vendor> vendors)
    {
        return new VendorListResponse
        {
            Items = vendors.ConvertAll(v => v.ToResponse())
        };
    }
}
