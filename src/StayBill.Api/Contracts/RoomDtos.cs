using System.ComponentModel.DataAnnotations;
using StayBill.Api.Domain;

namespace StayBill.Api.Contracts;

public sealed class CreateRoomRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public decimal? AreaM2 { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MonthlyRent { get; set; }
}

public sealed class UpdateRoomRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    public decimal? AreaM2 { get; set; }

    [Range(0, double.MaxValue)]
    public decimal MonthlyRent { get; set; }
}

public sealed record RoomResponse(
    Guid Id,
    string Code,
    decimal? AreaM2,
    decimal MonthlyRent,
    RoomStatus Status);
