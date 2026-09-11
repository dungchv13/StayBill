using Microsoft.EntityFrameworkCore;
using StayBill.Api.Contracts;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api.Services;

public sealed class TenantService
{
    private readonly StayBillDbContext _db;

    public TenantService(StayBillDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<TenantResponse>> ListAsync(CancellationToken ct)
    {
        var tenants = await _db.Tenants.AsNoTracking().OrderBy(x => x.FullName).ToListAsync(ct);
        return tenants.Select(ToResponse).ToList();
    }

    public async Task<TenantResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var tenant = await _db.Tenants.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (tenant is null)
        {
            throw AppException.NotFound("TENANT_NOT_FOUND", "Tenant was not found.");
        }

        return ToResponse(tenant);
    }

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken ct)
    {
        Validate(request.Phone, request.IdNumber);
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            IdNumber = request.IdNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(ct);
        return ToResponse(tenant);
    }

    public async Task<TenantResponse> UpdateAsync(Guid id, UpdateTenantRequest request, CancellationToken ct)
    {
        Validate(request.Phone, request.IdNumber);
        var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (tenant is null)
        {
            throw AppException.NotFound("TENANT_NOT_FOUND", "Tenant was not found.");
        }

        tenant.FullName = request.FullName.Trim();
        tenant.Phone = request.Phone.Trim();
        tenant.IdNumber = request.IdNumber.Trim();
        tenant.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        await _db.SaveChangesAsync(ct);
        return ToResponse(tenant);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (tenant is null)
        {
            throw AppException.NotFound("TENANT_NOT_FOUND", "Tenant was not found.");
        }

        var hasActive = await _db.Contracts.AnyAsync(
            x => x.TenantId == id && x.Status == ContractStatus.Active, ct);
        if (hasActive)
        {
            throw AppException.Conflict("TENANT_HAS_ACTIVE_CONTRACT", "Cannot delete a tenant with an active contract.");
        }

        _db.Tenants.Remove(tenant);
        await _db.SaveChangesAsync(ct);
    }

    private static void Validate(string phone, string idNumber)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            throw AppException.BadRequest("PHONE_REQUIRED", "Phone is required.");
        }

        if (string.IsNullOrWhiteSpace(idNumber))
        {
            throw AppException.BadRequest("ID_NUMBER_REQUIRED", "Id number is required.");
        }
    }

    private static TenantResponse ToResponse(Tenant tenant) =>
        new(tenant.Id, tenant.FullName, tenant.Phone, tenant.IdNumber, tenant.Email);
}
