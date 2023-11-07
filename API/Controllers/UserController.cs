using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Security.Interfaces;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IIdentityService _identityService;

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
            return Created("", "");

        return BadRequest(result.Errors);
    }

    [HttpGet("Confirmation")]
    public async Task<IActionResult> CreateUser([FromQuery] string userId, [FromQuery] string emailConfirmationToken,
        CancellationToken cancellationToken)
    {
        var result =
            await _identityService.UserEmailConfirmationToken(userId, emailConfirmationToken, cancellationToken);

        return result ? Ok("Token confirmado") : BadRequest("Token inválido");
    }

    [HttpGet("Password-Reset")]
    public async Task<IActionResult> PasswordReset(CancellationToken cancellationToken)
    {
        var user = User.Identity as ClaimsIdentity;
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return BadRequest();
        
        var result = await _identityService.PasswordReset(userId, cancellationToken);

        return result is true ? Ok("Token enviado") : BadRequest("Token não enviado");
    }
}