using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record ValidateTokenQuery(ValidateTokenRequest Request) : IRequest<ValidateTokenResponse>;

public class ValidateTokenQueryHandler : IRequestHandler<ValidateTokenQuery, ValidateTokenResponse>
{
    private readonly IConfiguration _configuration;

    public ValidateTokenQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<ValidateTokenResponse> Handle(ValidateTokenQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var token = query.Request.Token;

            // Remove "Bearer " prefix if present
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                token = token.Substring(7);

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtCredentials:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtCredentials:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtCredentials:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return Task.FromResult(new ValidateTokenResponse(
                    IsValid: false,
                    UserId: null,
                    Email: null,
                    Username: null,
                    FullName: null,
                    Roles: Array.Empty<string>(),
                    Permissions: Array.Empty<string>(),
                    ExpiresAt: null,
                    Error: "Token inválido"
                ));
            }

            var userId = principal.FindFirst("userId")?.Value;
            var email = principal.FindFirst("email")?.Value;
            var username = principal.FindFirst("userName")?.Value;
            var fullName = principal.FindFirst("name")?.Value;
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var permissionsString = principal.FindFirst("permisos")?.Value;
            var permissions = string.IsNullOrEmpty(permissionsString)
                ? Array.Empty<string>()
                : permissionsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            return Task.FromResult(new ValidateTokenResponse(
                IsValid: true,
                UserId: userId,
                Email: email,
                Username: username,
                FullName: fullName,
                Roles: roles,
                Permissions: permissions,
                ExpiresAt: jwtToken.ValidTo,
                Error: null
            ));
        }
        catch (SecurityTokenExpiredException)
        {
            return Task.FromResult(new ValidateTokenResponse(
                IsValid: false,
                UserId: null,
                Email: null,
                Username: null,
                FullName: null,
                Roles: Array.Empty<string>(),
                Permissions: Array.Empty<string>(),
                ExpiresAt: null,
                Error: "Token expirado"
            ));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new ValidateTokenResponse(
                IsValid: false,
                UserId: null,
                Email: null,
                Username: null,
                FullName: null,
                Roles: Array.Empty<string>(),
                Permissions: Array.Empty<string>(),
                ExpiresAt: null,
                Error: $"Token inválido: {ex.Message}"
            ));
        }
    }
}
