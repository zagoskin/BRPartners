using BRP.VendorManagement.Infrastructure.Persistence.OutboxIntegrationEvents;
using MediatR;

namespace BRP.VendorManagement.Infrastructure.JustATest;

public record ListOutboxIntegrationEventsQuery() : IRequest<List<OutboxIntegrationEvent>>;
