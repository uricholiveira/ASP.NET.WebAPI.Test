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
        var result = await _identityService.UserEmailConfirmationToken(userId, emailConfirmationToken, cancellationToken);

        return result ? Ok("Token confirmado") : BadRequest("Token inválido");
    }
}