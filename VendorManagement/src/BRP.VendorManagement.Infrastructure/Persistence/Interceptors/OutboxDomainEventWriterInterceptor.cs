using System.Text.Json;
using BRP.VendorManagement.Domain.Common.Models;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxDomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BRP.VendorManagement.Infrastructure.Persistence.Interceptors;
internal sealed class OutboxDomainEventWriterInterceptor : SaveChangesInterceptor
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);

    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public static async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken token = default)
    {
        if (context is null || context is not VendorManagementDbContext vendorManagementContext)
            return;

        var relevantEntities = vendorManagementContext.ChangeTracker.Entries<IHaveDomainEvents>()           
           .Where(entry => entry.Entity.DomainEvents.Any())
           .Select(entry => entry.Entity)
           .ToList();

        var domainEvents = relevantEntities
            .SelectMany(entity => entity.DomainEvents)
            .ToList();

        foreach (var entity in relevantEntities)
        {
            entity.ClearDomainEvents();
        }


        var outboxDomainEvents = domainEvents.ConvertAll(domainEvent =>
            new OutboxDomainEvent(
                domainEvent.GetType().Name,
                JsonSerializer.Serialize(
                    domainEvent, 
                    domainEvent.GetType(), 
                    _jsonSerializerOptions)));

        if (outboxDomainEvents.Count is not 0)
        {
            await vendorManagementContext.OutboxDomainEvents.AddRangeAsync(outboxDomainEvents, token);
        }
    }
}
