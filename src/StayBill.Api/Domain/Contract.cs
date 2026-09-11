namespace StayBill.Api.Domain;

public sealed class Contract
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid TenantId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal Deposit { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Active;
    public DateTimeOffset? EndedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Room Room { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
