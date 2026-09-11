using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StayBill.Api.Contracts;
using StayBill.Api.Services;

namespace StayBill.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _auth;

    public AuthController(AuthService auth)
    {
        _auth = auth;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var user = await _auth.RegisterAsync(request, ct);
        return Created($"/api/auth/register", user);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        return Ok(await _auth.LoginAsync(request, ct));
    }
}
