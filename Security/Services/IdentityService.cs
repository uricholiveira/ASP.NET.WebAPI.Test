using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Security.Helpers;
using Security.Interfaces;
using Security.Models;
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

    public async Task<JwtToken?> GenerateToken(IdentityUser user)
    {
        var claims = await GenerateClaims(user);
        var userClaims = await GenerateUserClaims(user);

        var accessTokenExpiration = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
        var refreshTokenExpiration = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

        var jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
            DateTime.Now, accessTokenExpiration, _jwtOptions.SigningCredentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        foreach (var userClaim in userClaims)
        {
            claims.Add(userClaim);
        }

        jwt = new JwtSecurityToken(_jwtOptions.Issuer, _jwtOptions.Audience, claims,
            DateTime.Now, refreshTokenExpiration, _jwtOptions.SigningCredentials);
        var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JwtToken
        {
            Expiration = _jwtOptions.AccessTokenExpiration,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public Task<IList<Claim>> GenerateClaims(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString(CultureInfo.InvariantCulture)),
            new(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString(CultureInfo.InvariantCulture))
        };

        return Task.FromResult<IList<Claim>>(claims);
    }

    public async Task<IList<Claim>> GenerateUserClaims(IdentityUser user)
    {
        var claims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);

        foreach (var role in roles) claims.Add(new Claim("role", role));

        return claims;
    }

    public async Task<JwtToken?> Login(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        // TODO: Adicionar validação de erros
        return await GenerateToken(user);
    }

    public async Task<JwtToken?> Login(LoginRequest data, CancellationToken cancellationToken)
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