using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayBill.Api.Domain;

namespace StayBill.Api.Data.Configurations;

public sealed class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contracts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        builder.Property(x => x.Deposit).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.Contracts)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.RoomId, x.Status });
    }
}
