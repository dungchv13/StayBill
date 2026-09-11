using System.ComponentModel.DataAnnotations;

namespace StayBill.Api.Contracts;

public sealed class CreateTenantRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string IdNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }
}

public sealed class UpdateTenantRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    public string IdNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }
}

public sealed record TenantResponse(
    Guid Id,
    string FullName,
    string Phone,
    string IdNumber,
    string? Email);
