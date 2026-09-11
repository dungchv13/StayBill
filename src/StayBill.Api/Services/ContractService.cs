using Microsoft.EntityFrameworkCore;
using StayBill.Api.Cache;
using StayBill.Api.Contracts;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api.Services;

public sealed class ContractService
{
    private readonly StayBillDbContext _db;
    private readonly IVacantRoomCache _cache;

    public ContractService(StayBillDbContext db, IVacantRoomCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ContractResponse>> ListAsync(Guid? roomId, ContractStatus? status, CancellationToken ct)
    {
        var query = _db.Contracts.AsNoTracking().AsQueryable();
        if (roomId is not null)
        {
            query = query.Where(x => x.RoomId == roomId);
        }

        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }

        var items = await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
        return items.Select(ToResponse).ToList();
    }

    public async Task<ContractResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var contract = await _db.Contracts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (contract is null)
        {
            throw AppException.NotFound("CONTRACT_NOT_FOUND", "Contract was not found.");
        }

        return ToResponse(contract);
    }

    public async Task<ContractResponse> CreateAsync(CreateContractRequest request, CancellationToken ct)
    {
        var room = await _db.Rooms.FirstOrDefaultAsync(x => x.Id == request.RoomId, ct);
        if (room is null)
        {
            throw AppException.NotFound("ROOM_NOT_FOUND", "Room was not found.");
        }

        var tenant = await _db.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.TenantId, ct);
        if (tenant is null)
        {
            throw AppException.NotFound("TENANT_NOT_FOUND", "Tenant was not found.");
        }

        var occupied = !OccupancyRules.CanCreateActiveContract(
            room.Status,
            await _db.Contracts.AnyAsync(x => x.RoomId == room.Id && x.Status == ContractStatus.Active, ct));
        if (occupied)
        {
            throw AppException.Conflict("ROOM_OCCUPIED", "Room already has an active contract.");
        }

        var contract = new Contract
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            TenantId = tenant.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MonthlyRent = request.MonthlyRent ?? room.MonthlyRent,
            Deposit = request.Deposit,
            Status = ContractStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };
        room.Status = RoomStatus.Occupied;
        _db.Contracts.Add(contract);
        await _db.SaveChangesAsync(ct);
        await _cache.InvalidateAsync(ct);
        return ToResponse(contract);
    }

    public async Task<ContractResponse> EndAsync(Guid id, CancellationToken ct)
    {
        var contract = await _db.Contracts.Include(x => x.Room).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (contract is null)
        {
            throw AppException.NotFound("CONTRACT_NOT_FOUND", "Contract was not found.");
        }

        if (contract.Status == ContractStatus.Ended)
        {
            throw AppException.Conflict("CONTRACT_ALREADY_ENDED", "Contract is already ended.");
        }

        contract.Status = ContractStatus.Ended;
        contract.EndedAt = DateTimeOffset.UtcNow;
        contract.Room.Status = RoomStatus.Vacant;
        await _db.SaveChangesAsync(ct);
        await _cache.InvalidateAsync(ct);
        return ToResponse(contract);
    }

    private static ContractResponse ToResponse(Contract contract) =>
        new(
            contract.Id,
            contract.RoomId,
            contract.TenantId,
            contract.StartDate,
            contract.EndDate,
            contract.MonthlyRent,
            contract.Deposit,
            contract.Status,
            contract.EndedAt);
}
