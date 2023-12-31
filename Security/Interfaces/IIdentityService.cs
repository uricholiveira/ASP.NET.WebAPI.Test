﻿using System.Security.Claims;
using Data.Models.Request;
using Microsoft.AspNetCore.Identity;
using Security.Models;

namespace Security.Interfaces;

public interface IIdentityService
{
    public Task<IdentityResult> CreateUser(CreateUserRequest data, CancellationToken cancellationToken);
    public Task<IdentityResult?> UpdatePassword(string userId, ResetPassword data, CancellationToken cancellationToken);
    public Task<bool?> ResetPassword(string userId, CancellationToken cancellationToken);

    public Task<bool> VerifyEmail(string userId, string emailConfirmationToken,
        CancellationToken cancellationToken);

    public Task<JwtToken?> GenerateToken(IdentityUser user);
    public Task<IList<Claim>> GenerateClaims(IdentityUser user);
    public Task<JwtToken?> Login(string userId, CancellationToken cancellationToken);
    public Task<JwtToken?> Login(LoginRequest data, CancellationToken cancellationToken);
}