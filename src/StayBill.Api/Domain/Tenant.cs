namespace StayBill.Api.Domain;

public sealed class Tenant
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string IdNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
