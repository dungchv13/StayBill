using System.ComponentModel.DataAnnotations;
using StayBill.Api.Domain;

namespace StayBill.Api.Contracts;

public sealed class CreateInvoiceRequest
{
    [Required]
    public Guid ContractId { get; set; }

    [Required]
    public string Period { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal ElectricityKwh { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ElectricityUnitPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal WaterM3 { get; set; }

    [Range(0, double.MaxValue)]
    public decimal WaterUnitPrice { get; set; }
}

public sealed record InvoiceResponse(
    Guid Id,
    Guid ContractId,
    string Period,
    decimal RentAmount,
    decimal ElectricityKwh,
    decimal ElectricityUnitPrice,
    decimal WaterM3,
    decimal WaterUnitPrice,
    decimal Total,
    InvoiceStatus Status,
    DateTimeOffset? PaidAt);
