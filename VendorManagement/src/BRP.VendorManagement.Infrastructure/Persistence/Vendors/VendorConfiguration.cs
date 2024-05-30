using BRP.VendorManagement.Domain.Common.ValueObjects;
using BRP.VendorManagement.Domain.VendorAggregate;
using BRP.VendorManagement.Domain.VendorAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BRP.VendorManagement.Infrastructure.Persistence.Vendors;
internal sealed class VendorConfiguration : AuditableEntityConfiguration<Vendor>
{
    public override void Configure(EntityTypeBuilder<Vendor> builder)
    {
        base.Configure(builder);

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .ValueGeneratedNever()
            .HasConversion(
                vendorId => vendorId.Value,
                guid => VendorId.Create(guid));

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.Identifier)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Email)
            .IsRequired()
            .HasConversion(
                email => email.Value,
                value => new EmailAddress(value));

        builder.HasData(VendorSeed.GetData());
    }
}
