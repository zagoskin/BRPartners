using BRP.VendorManagement.Domain.Common.Abstractions;
using BRP.VendorManagement.Domain.Common.ValueObjects;

namespace BRP.VendorManagement.Domain.Common.Models;

public abstract class AggregateRoot<TId> : Entity<TId>    
    where TId : notnull
{
    protected AggregateRoot(TId id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
