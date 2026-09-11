using Microsoft.EntityFrameworkCore;
using StayBill.Api.Domain;

namespace StayBill.Api.Data;

public sealed class StayBillDbContext : DbContext
{
    public StayBillDbContext(DbContextOptions<StayBillDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StayBillDbContext).Assembly);
    }
}
