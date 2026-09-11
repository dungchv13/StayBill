namespace StayBill.Api.Domain;

public sealed class Invoice
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public string Period { get; set; } = string.Empty;
    public decimal RentAmount { get; set; }
    public decimal ElectricityKwh { get; set; }
    public decimal ElectricityUnitPrice { get; set; }
    public decimal WaterM3 { get; set; }
    public decimal WaterUnitPrice { get; set; }
    public decimal Total { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTimeOffset? PaidAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public Contract Contract { get; set; } = null!;
}
