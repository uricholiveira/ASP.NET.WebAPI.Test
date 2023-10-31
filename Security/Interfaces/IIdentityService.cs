using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Identity;

namespace Security.Interfaces;

public interface IIdentityService
{
    public Task<IdentityResult> CreateUser(CreateUserRequest data, CancellationToken cancellationToken);
    public Task<string?> GenerateToken(string email);
    public Task<string?> GenerateToken(IdentityUser user);
    public Task<IList<Claim>> GenerateClaims(IdentityUser user);
    public Task<string?> Login(LoginRequest data, CancellationToken cancellationToken);
}