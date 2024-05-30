using ErrorOr;

namespace BRP.VendorManagement.Domain.ContractAggregate;

internal static class ContractErrors
{
    public static readonly Error TitleIsRequired = Error.Validation(
        code: "Contract.TitleIsRequired",
        description: "Title is required");

    public static readonly Error DeadLineMustBeInTheFuture = Error.Validation(
        code: "Contract.DeadLineMustBeInTheFuture",
        description: "DeadLine must be in the future");

    public static readonly Error EstimatedValueMustBeGreaterThanZero = Error.Validation(
        code: "Contract.EstimatedValueMustBeGreaterThanZero",
        description: "EstimatedValue must be greater than 0");
}
