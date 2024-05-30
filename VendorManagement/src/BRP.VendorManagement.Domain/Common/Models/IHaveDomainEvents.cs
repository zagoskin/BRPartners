using BRP.VendorManagement.Domain.Common.Abstractions;

namespace BRP.VendorManagement.Domain.Common.Models;

public interface IHaveDomainEvents 
{
    IReadOnlyList<IDomainEvent> DomainEvents { get; }
    void RaiseDomainEvent(IDomainEvent domainEvent);
    void ClearDomainEvents();
}
