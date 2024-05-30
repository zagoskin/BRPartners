using ErrorOr;

namespace BRP.VendorManagement.Domain.VendorAggregate;
internal static class VendorErrors
{
    public static readonly Error NameRequired = Error.Validation(
        code: "Vendor.NameRequired",
        description: "Vendor name is required.");

    public static readonly Error IdentifierRequired = Error.Validation(
        code: "Vendor.IdentifierRequired",
        description: "Vendor identifier is required.");
}
