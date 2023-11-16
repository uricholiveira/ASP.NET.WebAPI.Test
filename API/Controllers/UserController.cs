using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpGet("verify-email")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromQuery] string userId,
        [FromQuery] string emailConfirmationToken,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.VerifyEmail(userId, emailConfirmationToken, cancellationToken);

        return result ? Ok("Email confirmado") : BadRequest("Email não confirmado");
    }

    [HttpGet("reset-password")]
    public async Task<IActionResult> ResetPassword(CancellationToken cancellationToken)
    {
        var user = User.Identity as ClaimsIdentity;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest();

        var result = await _identityService.ResetPassword(userId, cancellationToken);

        return result is true
            ? Ok("Email de redefinição enviado")
            : BadRequest("Erro ao enviar o email de redefinição de senha");
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] ResetPassword data, CancellationToken cancellationToken)
    {
        var user = User.Identity as ClaimsIdentity;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest();

        var result = await _identityService.UpdatePassword(userId, data, cancellationToken);

        return result!.Succeeded
            ? Ok()
            : BadRequest("Erro ao redefinir senha");
    }
}