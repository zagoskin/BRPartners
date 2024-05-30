using BRP.VendorManagement.Infrastructure.Persistence;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxIntegrationEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BRP.VendorManagement.Infrastructure.JustATest;

internal sealed class ListOutboxIntegrationEventsQueryHandler : IRequestHandler<ListOutboxIntegrationEventsQuery, List<OutboxIntegrationEvent>>
{
    private readonly VendorManagementDbContext _context;

    public ListOutboxIntegrationEventsQueryHandler(VendorManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<OutboxIntegrationEvent>> Handle(ListOutboxIntegrationEventsQuery request, CancellationToken cancellationToken)
    {
        return await _context.OutboxIntegrationEvents
            .AsNoTracking()
            .OrderBy(e => e.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }
}
