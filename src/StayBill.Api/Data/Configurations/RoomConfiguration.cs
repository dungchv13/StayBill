using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StayBill.Api.Domain;

namespace StayBill.Api.Data.Configurations;

public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("rooms");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.HasIndex(x => x.Code).IsUnique();
        builder.Property(x => x.AreaM2).HasPrecision(18, 2);
        builder.Property(x => x.MonthlyRent).HasPrecision(18, 2);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
    }
}
