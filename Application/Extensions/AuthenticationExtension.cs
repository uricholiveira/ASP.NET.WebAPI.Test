using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Security.Helpers;

namespace Application.Extensions;

public static class AuthenticationExtension
{
    public static void AddCustomAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var jwtConfiguration = configuration.GetSection(nameof(JwtOptions));
        var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfiguration["SecurityKey"]!));


        serviceCollection.Configure<JwtOptions>(options =>
        {
            options.Issuer = jwtConfiguration[nameof(JwtOptions.Issuer)]!;
            options.Audience = jwtConfiguration[nameof(JwtOptions.Audience)]!;
            options.SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
            options.AccessTokenExpiration = int.Parse(jwtConfiguration[nameof(JwtOptions.AccessTokenExpiration)]!);
            options.RefreshTokenExpiration = int.Parse(jwtConfiguration[nameof(JwtOptions.RefreshTokenExpiration)]!);
        });

        serviceCollection.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        });

        var tokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfiguration.GetSection(nameof(JwtOptions.Issuer)).Value,
            ValidateAudience = true,
            ValidAudience = jwtConfiguration.GetSection(nameof(JwtOptions.Audience)).Value,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        serviceCollection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options => { options.TokenValidationParameters = tokenValidationParams; });
    }
}