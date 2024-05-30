using BRP.VendorManagement.Domain.Common.Models;
using BRP.VendorManagement.Domain.Common.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using ErrorOr;

namespace BRP.VendorManagement.Domain.VendorAggregate;
public sealed class Vendor : AggregateRoot<VendorId>
{
    public string Name { get; private set; } = null!;
    public EmailAddress Email { get; private set; } = null!;
    public string Identifier { get; private set; } = null!;
    
    internal Vendor(
        string name, 
        EmailAddress email, 
        string identifier, VendorId? id = null) : base(id ?? VendorId.CreateUnique())
    {        
        Name = name;
        Email = email;
        Identifier = identifier;
    }

    public static ErrorOr<Vendor> Create(string name, EmailAddress email, string identifier, VendorId? id = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return VendorErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(identifier))
        {
            return VendorErrors.IdentifierRequired;
        }

        return new Vendor(name, email, identifier, id);
    }

    private Vendor()
    {
    }
}
