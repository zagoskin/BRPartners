using ErrorOr;

namespace BRP.VendorManagement.Application.Common;
internal static class GeneralErrors
{
    public static Error NotFound<TIdType>(TIdType id, Type type) =>
        Error.NotFound(description: $"{type.Name} with id {id} not found");
}
