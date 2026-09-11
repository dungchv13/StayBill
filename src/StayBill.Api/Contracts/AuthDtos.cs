using System.ComponentModel.DataAnnotations;

namespace StayBill.Api.Contracts;

public sealed class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed record UserResponse(Guid Id, string Email, string FullName);

public sealed record LoginResponse(string AccessToken, DateTimeOffset ExpiresAt);
