using BRP.VendorManagement.Infrastructure.Persistence;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxDomainEvents;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BRP.VendorManagement.Infrastructure.JustATest;

internal sealed class ListOutboxDomainEventsQueryHandler : IRequestHandler<ListOutboxDomainEventsQuery, List<OutboxDomainEvent>>
{
    private readonly VendorManagementDbContext _context;

    public ListOutboxDomainEventsQueryHandler(VendorManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<OutboxDomainEvent>> Handle(ListOutboxDomainEventsQuery request, CancellationToken cancellationToken)
    {
        return await _context.OutboxDomainEvents
            .AsNoTracking()
            .OrderBy(e => e.CreatedOnUtc)
            .ToListAsync(cancellationToken);
    }
}
