namespace StayBill.Api.Domain;

public sealed class Room
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public decimal? AreaM2 { get; set; }
    public decimal MonthlyRent { get; set; }
    public RoomStatus Status { get; set; } = RoomStatus.Vacant;
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
