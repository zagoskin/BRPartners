using BRP.VendorManagement.Infrastructure.Persistence.OutboxDomainEvents;
using MediatR;

namespace BRP.VendorManagement.Infrastructure.JustATest;
public record ListOutboxDomainEventsQuery() : IRequest<List<OutboxDomainEvent>>;
