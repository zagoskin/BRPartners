namespace BRP.VendorManagement.Domain.Common.Abstractions;

public interface IAuditableEntity
{
    DateTime DateCreatedUtc { get; }
    DateTime? DateModifiedUtc { get; }
}
