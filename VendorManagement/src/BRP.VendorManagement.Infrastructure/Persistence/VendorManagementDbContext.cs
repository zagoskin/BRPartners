using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxDomainEvents;
using BRP.VendorManagement.Infrastructure.Persistence.OutboxIntegrationEvents;
using Microsoft.EntityFrameworkCore;

namespace BRP.VendorManagement.Infrastructure.Persistence;
internal sealed class VendorManagementDbContext : DbContext
{
    public DbSet<OutboxIntegrationEvent> OutboxIntegrationEvents => Set<OutboxIntegrationEvent>();
    public DbSet<OutboxDomainEvent> OutboxDomainEvents => Set<OutboxDomainEvent>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<Contract> Contracts => Set<Contract>();

    public VendorManagementDbContext(DbContextOptions<VendorManagementDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendorManagementDbContext).Assembly);
    }
}
