using Microsoft.EntityFrameworkCore;
using StayBill.Api.Cache;
using StayBill.Api.Contracts;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api.Services;

public sealed class RoomService
{
    private readonly StayBillDbContext _db;
    private readonly IVacantRoomCache _cache;

    public RoomService(StayBillDbContext db, IVacantRoomCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IReadOnlyList<RoomResponse>> ListAsync(CancellationToken ct)
    {
        var rooms = await _db.Rooms.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);
        return rooms.Select(ToResponse).ToList();
    }

    public async Task<IReadOnlyList<RoomResponse>> ListVacantAsync(CancellationToken ct)
    {
        var cached = await _cache.GetAsync(ct);
        if (cached is not null)
        {
            return cached;
        }

        var rooms = await _db.Rooms.AsNoTracking()
            .Where(x => x.Status == RoomStatus.Vacant)
            .OrderBy(x => x.Code)
            .ToListAsync(ct);
        var response = rooms.Select(ToResponse).ToList();
        await _cache.SetAsync(response, ct);
        return response;
    }

    public async Task<RoomResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var room = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (room is null)
        {
            throw AppException.NotFound("ROOM_NOT_FOUND", "Room was not found.");
        }

        return ToResponse(room);
    }

    public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken ct)
    {
        var code = request.Code.Trim();
        if (await _db.Rooms.AnyAsync(x => x.Code == code, ct))
        {
            throw AppException.Conflict("ROOM_CODE_TAKEN", "Room code is already used.");
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Code = code,
            AreaM2 = request.AreaM2,
            MonthlyRent = request.MonthlyRent,
            Status = RoomStatus.Vacant,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync(ct);
        await _cache.InvalidateAsync(ct);
        return ToResponse(room);
    }

    public async Task<RoomResponse> UpdateAsync(Guid id, UpdateRoomRequest request, CancellationToken ct)
    {
        var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (room is null)
        {
            throw AppException.NotFound("ROOM_NOT_FOUND", "Room was not found.");
        }

        var code = request.Code.Trim();
        if (await _db.Rooms.AnyAsync(x => x.Code == code && x.Id != id, ct))
        {
            throw AppException.Conflict("ROOM_CODE_TAKEN", "Room code is already used.");
        }

        room.Code = code;
        room.AreaM2 = request.AreaM2;
        room.MonthlyRent = request.MonthlyRent;
        await _db.SaveChangesAsync(ct);
        await _cache.InvalidateAsync(ct);
        return ToResponse(room);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (room is null)
        {
            throw AppException.NotFound("ROOM_NOT_FOUND", "Room was not found.");
        }

        var hasActive = await _db.Contracts.AnyAsync(
            x => x.RoomId == id && x.Status == ContractStatus.Active, ct);
        if (hasActive || room.Status == RoomStatus.Occupied)
        {
            throw AppException.Conflict("ROOM_HAS_ACTIVE_CONTRACT", "Cannot delete a room with an active contract.");
        }

        _db.Rooms.Remove(room);
        await _db.SaveChangesAsync(ct);
        await _cache.InvalidateAsync(ct);
    }

    public static RoomResponse ToResponse(Room room) =>
        new(room.Id, room.Code, room.AreaM2, room.MonthlyRent, room.Status);
}
