namespace BRP.VendorManagement.Domain.Common.Models;

public interface IAuditableEntity
{
    DateTime DateCreatedUtc { get; }
    DateTime? DateModifiedUtc { get; }
}
