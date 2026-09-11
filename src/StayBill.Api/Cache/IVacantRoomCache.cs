using StayBill.Api.Contracts;

namespace StayBill.Api.Cache;

public interface IVacantRoomCache
{
    Task<IReadOnlyList<RoomResponse>?> GetAsync(CancellationToken ct);
    Task SetAsync(IReadOnlyList<RoomResponse> rooms, CancellationToken ct);
    Task InvalidateAsync(CancellationToken ct);
}
