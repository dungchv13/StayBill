using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StayBill.Api.Auth;
using StayBill.Api.Contracts;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api.Services;

public sealed class AuthService
{
    private readonly StayBillDbContext _db;
    private readonly JwtTokenFactory _tokens;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(StayBillDbContext db, JwtTokenFactory tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public async Task<UserResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await _db.Users.AnyAsync(x => x.Email == email, ct);
        if (exists)
        {
            throw AppException.Conflict("EMAIL_TAKEN", "Email is already registered.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = request.FullName.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        return new UserResponse(user.Id, user.Email, user.FullName);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == email, ct);
        if (user is null)
        {
            throw AppException.Unauthorized("INVALID_CREDENTIALS", "Invalid email or password.");
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw AppException.Unauthorized("INVALID_CREDENTIALS", "Invalid email or password.");
        }

        var (token, expiresAt) = _tokens.Create(user);
        return new LoginResponse(token, expiresAt);
    }
}
