using BRP.VendorManagement.Domain.Common.Models;

namespace BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
public sealed class VendorId : ValueObject
{    
    public Guid Value { get; private set; }

    private VendorId(Guid id)
    {
        Value = id;
    }

    public static VendorId Create(Guid id)
    {
        return new VendorId(id);
    }

    public static VendorId CreateUnique()
    {
        return new VendorId(Guid.NewGuid());
    }


    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;    
    }
}
