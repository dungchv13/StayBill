using System.ComponentModel.DataAnnotations;
using StayBill.Api.Domain;

namespace StayBill.Api.Contracts;

public sealed class CreateContractRequest
{
    [Required]
    public Guid RoomId { get; set; }

    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? MonthlyRent { get; set; }

    public decimal Deposit { get; set; }
}

public sealed record ContractResponse(
    Guid Id,
    Guid RoomId,
    Guid TenantId,
    DateOnly StartDate,
    DateOnly? EndDate,
    decimal MonthlyRent,
    decimal Deposit,
    ContractStatus Status,
    DateTimeOffset? EndedAt);
