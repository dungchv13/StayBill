using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayBill.Api.Contracts;
using StayBill.Api.Domain;
using StayBill.Api.Services;

namespace StayBill.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/contracts")]
public sealed class ContractsController : ControllerBase
{
    private readonly ContractService _contracts;

    public ContractsController(ContractService contracts)
    {
        _contracts = contracts;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ContractResponse>>> List(
        [FromQuery] Guid? roomId,
        [FromQuery] ContractStatus? status,
        CancellationToken ct) =>
        Ok(await _contracts.ListAsync(roomId, status, ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(await _contracts.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<ContractResponse>> Create(CreateContractRequest request, CancellationToken ct)
    {
        var contract = await _contracts.CreateAsync(request, ct);
        return Created($"/api/contracts/{contract.Id}", contract);
    }

    [HttpPost("{id:guid}/end")]
    public async Task<ActionResult<ContractResponse>> End(Guid id, CancellationToken ct) =>
        Ok(await _contracts.EndAsync(id, ct));
}
