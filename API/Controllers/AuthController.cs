using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest data, CancellationToken cancellationToken)
    {
        var result = await _identityService.Login(data, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("Login/Refresh")]
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