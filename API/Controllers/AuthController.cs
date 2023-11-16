using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(CreateUserRequest data, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUser(data, cancellationToken);

        if (result.Succeeded)
            return Created("", result);

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest data, CancellationToken cancellationToken)
    {
        var result = await _identityService.Login(data, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("login/refresh")]
    public async Task<IActionResult> LoginRefresh(CancellationToken cancellationToken)
    {
        var user = User.Identity as ClaimsIdentity;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest();

        var result = await _identityService.Login(userId, cancellationToken);

        return Ok(result);
    }
}