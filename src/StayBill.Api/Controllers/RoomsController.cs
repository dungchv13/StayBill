using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayBill.Api.Contracts;
using StayBill.Api.Services;

namespace StayBill.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/rooms")]
public sealed class RoomsController : ControllerBase
{
    private readonly RoomService _rooms;

    public RoomsController(RoomService rooms)
    {
        _rooms = rooms;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> List(CancellationToken ct) =>
        Ok(await _rooms.ListAsync(ct));

    [HttpGet("vacant")]
    public async Task<ActionResult<IReadOnlyList<RoomResponse>>> Vacant(CancellationToken ct) =>
        Ok(await _rooms.ListVacantAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> Get(Guid id, CancellationToken ct) =>
        Ok(await _rooms.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Create(CreateRoomRequest request, CancellationToken ct)
    {
        var room = await _rooms.CreateAsync(request, ct);
        return Created($"/api/rooms/{room.Id}", room);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RoomResponse>> Update(Guid id, UpdateRoomRequest request, CancellationToken ct) =>
        Ok(await _rooms.UpdateAsync(id, request, ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _rooms.DeleteAsync(id, ct);
        return NoContent();
    }
}
