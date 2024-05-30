using BRP.VendorManagement.Domain.ContractAggregate;
using BRP.VendorManagement.Domain.ContractAggregate.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRP.VendorManagement.Infrastructure.Persistence.Contracts;
internal class ContractConfiguration : AuditableEntityConfiguration<Contract>
{
    public override void Configure(EntityTypeBuilder<Contract> builder)
    {
        base.Configure(builder);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever()
            .HasConversion(
                contractId => contractId.Value,
                guid => ContractId.Create(guid));

        builder.Property(c => c.VendorId)
            .ValueGeneratedNever()
            .HasConversion(
                vendorId => vendorId.Value,
                guid => VendorId.Create(guid));        

        builder.HasOne<Vendor>()
            .WithMany()
            .HasForeignKey(c => c.VendorId);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.DeadLineUtc)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(c => c.EstimatedValue)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion(
                status => status.ToString(),
                value => (ContractStatus)Enum.Parse(typeof(ContractStatus), value));

        builder.OwnsMany(c => c.ContractHistories, chBuilder =>
        {
            chBuilder.HasKey(ch => ch.Id);
            chBuilder.Property(ch => ch.Id)
                .ValueGeneratedNever();

            chBuilder.WithOwner().HasForeignKey("ContractId");

            chBuilder.Property(ch => ch.Actor)
                .IsRequired()
                .HasMaxLength(255);

            chBuilder.Property(ch => ch.Message)
                .IsRequired()
                .HasMaxLength(255);

            chBuilder.Property(c => c.Status)
                .IsRequired()
                .HasConversion(
                    status => status.ToString(),
                    value => (ContractStatus)Enum.Parse(typeof(ContractStatus), value));

            chBuilder.Property(e => e.DateCreatedUtc)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

            chBuilder.Property(e => e.DateModifiedUtc)
                .HasColumnType("datetime2");
        })
        .Metadata
        .SetPropertyAccessMode(PropertyAccessMode.Field);
}
}
