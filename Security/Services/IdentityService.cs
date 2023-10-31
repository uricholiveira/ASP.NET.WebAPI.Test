using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Security.Helpers;
using Security.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Security.Services;

public class IdentityService : IIdentityService
{
    private readonly JwtOptions _jwtOptions;
    private readonly IPasswordHasher<IdentityUser> _passwordHasher;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public IdentityService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
        IOptions<JwtOptions> jwtOptions, IPasswordHasher<IdentityUser> passwordHasher)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
        _passwordHasher = passwordHasher;
    }

    public async Task<IdentityResult> CreateUser(CreateUserRequest data, CancellationToken cancellationToken)
    {
        var user = new IdentityUser
        {
            UserName = data.Email,
            Email = data.Email,
            EmailConfirmed = false
        };

        return await _userManager.CreateAsync(user, data.Password);
    }

    public async Task<string?> GenerateToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        var claims = await GenerateClaims(user);
        var expiration = DateTime.Now.AddSeconds(_jwtOptions.Expiration);

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
            DateTime.Now, expiration, _jwtOptions.SigningCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }

    public async Task<string?> GenerateToken(IdentityUser user)
    {
        var claims = await GenerateClaims(user);
        var expiration = DateTime.Now.AddSeconds(_jwtOptions.Expiration);

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
            DateTime.Now, expiration, _jwtOptions.SigningCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return token;
    }

    public async Task<IList<Claim>> GenerateClaims(IdentityUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString(CultureInfo.InvariantCulture)));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString(CultureInfo.InvariantCulture)));

        foreach (var role in roles) claims.Add(new Claim("role", role));

        return claims;
    }

    public async Task<string?> Login(LoginRequest data, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(data.Email);
        if (user is null)
            return null;

        var result = await _signInManager.PasswordSignInAsync(user, data.Password, false, false);
        if (result.Succeeded)
            return await GenerateToken(user);

        // TODO: Adicionar validação de erros
        return null;
    }
}