using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using StayBill.Api.Contracts;
using StayBill.Api.Data;
using StayBill.Api.Domain;

namespace StayBill.Api.Services;

public sealed class InvoiceService
{
    private static readonly Regex PeriodPattern = new(@"^\d{4}-(0[1-9]|1[0-2])$", RegexOptions.Compiled);

    private readonly StayBillDbContext _db;

    public InvoiceService(StayBillDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<InvoiceResponse>> ListAsync(Guid? contractId, InvoiceStatus? status, CancellationToken ct)
    {
        var query = _db.Invoices.AsNoTracking().AsQueryable();
        if (contractId is not null)
        {
            query = query.Where(x => x.ContractId == contractId);
        }

        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }

        var items = await query.OrderByDescending(x => x.Period).ToListAsync(ct);
        return items.Select(ToResponse).ToList();
    }

    public async Task<InvoiceResponse> GetAsync(Guid id, CancellationToken ct)
    {
        var invoice = await _db.Invoices.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice is null)
        {
            throw AppException.NotFound("INVOICE_NOT_FOUND", "Invoice was not found.");
        }

        return ToResponse(invoice);
    }

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, CancellationToken ct)
    {
        if (!PeriodPattern.IsMatch(request.Period))
        {
            throw AppException.BadRequest("INVALID_PERIOD", "Period must be yyyy-MM.");
        }

        var contract = await _db.Contracts.FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);
        if (contract is null)
        {
            throw AppException.NotFound("CONTRACT_NOT_FOUND", "Contract was not found.");
        }

        if (contract.Status != ContractStatus.Active)
        {
            throw AppException.Conflict("CONTRACT_NOT_ACTIVE", "Invoices can only be created for an active contract.");
        }

        var exists = await _db.Invoices.AnyAsync(
            x => x.ContractId == contract.Id && x.Period == request.Period, ct);
        if (exists)
        {
            throw AppException.Conflict("INVOICE_PERIOD_EXISTS", "An invoice already exists for this period.");
        }

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            ContractId = contract.Id,
            Period = request.Period,
            RentAmount = contract.MonthlyRent,
            ElectricityKwh = request.ElectricityKwh,
            ElectricityUnitPrice = request.ElectricityUnitPrice,
            WaterM3 = request.WaterM3,
            WaterUnitPrice = request.WaterUnitPrice,
            Total = InvoiceCalculator.Total(
                contract.MonthlyRent,
                request.ElectricityKwh,
                request.ElectricityUnitPrice,
                request.WaterM3,
                request.WaterUnitPrice),
            Status = InvoiceStatus.Unpaid,
            CreatedAt = DateTimeOffset.UtcNow
        };
        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync(ct);
        return ToResponse(invoice);
    }

    public async Task<InvoiceResponse> PayAsync(Guid id, CancellationToken ct)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (invoice is null)
        {
            throw AppException.NotFound("INVOICE_NOT_FOUND", "Invoice was not found.");
        }

        if (invoice.Status == InvoiceStatus.Paid)
        {
            throw AppException.Conflict("INVOICE_ALREADY_PAID", "Invoice is already paid.");
        }

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ToResponse(invoice);
    }

    private static InvoiceResponse ToResponse(Invoice invoice) =>
        new(
            invoice.Id,
            invoice.ContractId,
            invoice.Period,
            invoice.RentAmount,
            invoice.ElectricityKwh,
            invoice.ElectricityUnitPrice,
            invoice.WaterM3,
            invoice.WaterUnitPrice,
            invoice.Total,
            invoice.Status,
            invoice.PaidAt);
}
