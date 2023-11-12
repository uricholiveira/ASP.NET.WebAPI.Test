using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest data, CancellationToken cancellationToken)
    {
        var result = await _identityService.CreateUser(data, cancellationToken);

        if (result.Succeeded)
            return Created("", result);

        return BadRequest(result.Errors);
    }

    [HttpGet("Email/Confirmation")]
    [AllowAnonymous]
    public async Task<IActionResult> EmailConfirmation([FromQuery] string userId,
        [FromQuery] string emailConfirmationToken,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.ConfirmEmail(userId, emailConfirmationToken, cancellationToken);

        return result ? Ok("Email confirmado") : BadRequest("Confirmação inválida");
    }

    [HttpGet("Password/Reset")]
    public async Task<IActionResult> ResetPassword(CancellationToken cancellationToken)
    {
        var user = User.Identity as ClaimsIdentity;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest();

        var result = await _identityService.GeneratePasswordResetToken(userId, cancellationToken);

        return result is true
            ? Ok("Email de redefinição enviado")
            : BadRequest("Erro ao enviar o email de redefinição de senha");
    }

    [HttpPost("Password/Reset")]
    public async Task<IActionResult> ChangePassword([FromBody] ResetPassword data, CancellationToken cancellationToken)
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