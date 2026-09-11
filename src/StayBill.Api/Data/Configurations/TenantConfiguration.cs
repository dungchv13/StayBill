using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayBill.Api.Domain;

namespace StayBill.Api.Data.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("tenants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(32).IsRequired();
        builder.Property(x => x.IdNumber).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256);
    }
}
