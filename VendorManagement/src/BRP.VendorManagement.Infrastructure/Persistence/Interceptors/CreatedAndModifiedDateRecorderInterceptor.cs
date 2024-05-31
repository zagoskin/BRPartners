using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BRP.VendorManagement.Domain.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BRP.VendorManagement.Infrastructure.Persistence.Interceptors;
internal class CreatedAndModifiedDateRecorderInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        RecordDateChanges(eventData.Context);

        return base.SavingChanges(eventData, result);

    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        RecordDateChanges(eventData.Context, cancellationToken);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void RecordDateChanges(DbContext? context, CancellationToken token = default)
    {
        if (context is null || context is not VendorManagementDbContext vendorManagementContext)
            return;

        var now = DateTime.UtcNow;
        var entries = context.ChangeTracker
                .Entries<IAuditableEntity>()
                .Where(e => e.State == EntityState.Added
                || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(e => e.DateCreatedUtc).CurrentValue = now;
                continue;
            }

            entry.Property(e => e.DateModifiedUtc).CurrentValue = now;
        }
    }
}
