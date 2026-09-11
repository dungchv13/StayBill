using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api;

public static class DevSeed
{
    public static async Task EnsureAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StayBillDbContext>();
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "admin@staybill.local",
            FullName = "StayBill Admin",
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = hasher.HashPassword(user, "Admin123!");

        db.Users.Add(user);
        db.Rooms.AddRange(
            new Room
            {
                Id = Guid.NewGuid(),
                Code = "P101",
                AreaM2 = 18,
                MonthlyRent = 3_500_000,
                Status = RoomStatus.Vacant,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Room
            {
                Id = Guid.NewGuid(),
                Code = "P102",
                AreaM2 = 22,
                MonthlyRent = 4_200_000,
                Status = RoomStatus.Vacant,
                CreatedAt = DateTimeOffset.UtcNow
            });
        db.Tenants.Add(new Tenant
        {
            Id = Guid.NewGuid(),
            FullName = "Tran Thi B",
            Phone = "0901234567",
            IdNumber = "001234567890",
            Email = "b@example.com",
            CreatedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
