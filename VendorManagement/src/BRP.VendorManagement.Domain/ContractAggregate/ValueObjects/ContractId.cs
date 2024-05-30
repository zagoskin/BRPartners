using System.Text.Json.Serialization;
using BRP.VendorManagement.Domain.Common.Models;
using BRP.VendorManagement.Domain.Common.ValueObjects;

namespace BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
public sealed class ContractId : ValueObject
{
    [JsonInclude]
    [JsonPropertyName("value")]
    public Guid Value { get; private set; }
    
    private ContractId(Guid value)
    {
        Value = value;
    }        

    public static ContractId Create(Guid id)
    {
        return new ContractId(id);
    }

    public static ContractId CreateUnique()
    {
        return new ContractId(Guid.NewGuid());
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
