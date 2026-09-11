using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayBill.Api.Contracts;
using StayBill.Api.Services;

namespace StayBill.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tenants")]
public sealed class TenantsController : ControllerBase
{
    private readonly TenantService _tenants;

    public TenantsController(TenantService tenants)
    {
        _tenants = tenants;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TenantResponse>>> List(CancellationToken ct) =>
        Ok(await _tenants.ListAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TenantResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(await _tenants.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<TenantResponse>> Create(CreateTenantRequest request, CancellationToken ct)
    {
        var tenant = await _tenants.CreateAsync(request, ct);
        return Created($"/api/tenants/{tenant.Id}", tenant);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TenantResponse>> Update(Guid id, UpdateTenantRequest request, CancellationToken ct) =>
        Ok(await _tenants.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _tenants.DeleteAsync(id, ct);
        return NoContent();
    }
}
