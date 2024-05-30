using BRP.VendorManagement.Domain.Common.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;

namespace BRP.VendorManagement.Infrastructure.Persistence.Vendors;
internal static class VendorSeed
{
    public static List<Vendor> GetData()
    {
        return new List<Vendor>
        {
            Vendor.Create("Vendor 1", new EmailAddress("vendor1@test.com"), "111111111", VendorId.Create(Guid.Parse("992A4F6A-4010-44EC-83B2-3718BB9E6E58"))).Value,
            Vendor.Create("Vendor 2", new EmailAddress("vendor2@test.com"), "222222222", VendorId.Create(Guid.Parse("3A8AF5A0-9EB0-4773-8036-B2E2D4FFA08E"))).Value,
            Vendor.Create("Vendor 3", new EmailAddress("vendor3@test.com"), "333333333", VendorId.Create(Guid.Parse("CE704233-A7CE-48E0-93BD-5819EC06A4F3"))).Value,
        };
    }
}
