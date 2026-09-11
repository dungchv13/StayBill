using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayBill.Api.Contracts;
using StayBill.Api.Domain;
using StayBill.Api.Services;

namespace StayBill.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/invoices")]
public sealed class InvoicesController : ControllerBase
{
    private readonly InvoiceService _invoices;

    public InvoicesController(InvoiceService invoices)
    {
        _invoices = invoices;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<InvoiceResponse>>> List(
        [FromQuery] Guid? contractId,
        [FromQuery] InvoiceStatus? status,
        CancellationToken ct) =>
        Ok(await _invoices.ListAsync(contractId, status, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(await _invoices.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create(CreateInvoiceRequest request, CancellationToken ct)
    {
        var invoice = await _invoices.CreateAsync(request, ct);
        return Created($"/api/invoices/{invoice.Id}", invoice);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<ActionResult<InvoiceResponse>> Pay(Guid id, CancellationToken ct) =>
        Ok(await _invoices.PayAsync(id, ct));
}
