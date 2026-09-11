using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayBill.Api.Domain;

namespace StayBill.Api.Data.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Period).HasMaxLength(7).IsRequired();
        builder.Property(x => x.RentAmount).HasPrecision(18, 2);
        builder.Property(x => x.ElectricityKwh).HasPrecision(18, 2);
        builder.Property(x => x.ElectricityUnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.WaterM3).HasPrecision(18, 2);
        builder.Property(x => x.WaterUnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Total).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);

        builder.HasOne(x => x.Contract)
            .WithMany(x => x.Invoices)
            .HasForeignKey(x => x.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ContractId, x.Period }).IsUnique();
    }
}
