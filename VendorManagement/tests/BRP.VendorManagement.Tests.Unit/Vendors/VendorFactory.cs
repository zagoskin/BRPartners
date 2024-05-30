using BRP.VendorManagement.Domain.Common.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;

namespace BRP.VendorManagement.Tests.Unit.Vendors;

public static class VendorFactory
{
    private const string ValidName = "Totonet";
    private const string ValidEmail = "totonet@test.com";
    private const string ValidIdentifier = "70707070";

    public static Vendor CreateValidVendor(VendorId? vendorId = null)
    {
        return new Vendor(
            ValidName,
            new EmailAddress(ValidEmail),
            ValidIdentifier,
            vendorId);
    }
}
