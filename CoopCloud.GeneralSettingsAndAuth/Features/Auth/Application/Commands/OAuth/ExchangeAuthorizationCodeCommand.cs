using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.OAuth;

public record ExchangeAuthorizationCodeCommand(
    string Code,
    string RedirectUri,
    string ClientId,
    string? ClientSecret,
    string? CodeVerifier
) : IRequest<TokenResponse>;

public class ExchangeAuthorizationCodeCommandHandler : IRequestHandler<ExchangeAuthorizationCodeCommand, TokenResponse>
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public ExchangeAuthorizationCodeCommandHandler(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenResponse> Handle(ExchangeAuthorizationCodeCommand command, CancellationToken cancellationToken)
    {
        // Find the authorization code
        var authCode = await _context.Set<AuthorizationCode>()
            .Include(ac => ac.Client)
            .Include(ac => ac.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.Permissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(ac => ac.Code == command.Code, cancellationToken);

        if (authCode == null)
            throw new UnauthorizedAccessException("Código de autorización inválido");

        if (authCode.IsUsed)
            throw new UnauthorizedAccessException("El código de autorización ya fue utilizado");

        if (authCode.IsExpired)
            throw new UnauthorizedAccessException("El código de autorización ha expirado");

        // Validate client
        var client = authCode.Client;
        if (client.ClientId != command.ClientId)
            throw new UnauthorizedAccessException("El client_id no coincide");

        // Validate client secret for confidential clients
        if (client.ClientType == "confidential")
        {
            if (string.IsNullOrEmpty(command.ClientSecret) || client.ClientSecret != command.ClientSecret)
                throw new UnauthorizedAccessException("Client secret inválido");
        }

        // Validate redirect URI
        if (authCode.RedirectUri != command.RedirectUri)
            throw new UnauthorizedAccessException("El redirect_uri no coincide");

        // Validate PKCE if code challenge was provided
        if (!string.IsNullOrEmpty(authCode.CodeChallenge))
        {
            if (string.IsNullOrEmpty(command.CodeVerifier))
                throw new UnauthorizedAccessException("Se requiere code_verifier");

            var isValidPkce = ValidatePkce(command.CodeVerifier, authCode.CodeChallenge, authCode.CodeChallengeMethod);
            if (!isValidPkce)
                throw new UnauthorizedAccessException("code_verifier inválido");
        }

        // Mark code as used
        authCode.IsUsed = true;
        authCode.UsedAt = DateTime.UtcNow;

        var user = authCode.User;

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var accessToken = GenerateAccessToken(user, client, authCode.Scopes);
        var refreshToken = await GenerateRefreshToken(user.Id, cancellationToken);
        var idToken = GenerateIdToken(user, client, authCode.Scopes, authCode.Nonce);

        await _context.SaveChangesAsync(cancellationToken);

        return new TokenResponse(
            AccessToken: accessToken,
            TokenType: "Bearer",
            ExpiresIn: client.AccessTokenLifetimeMinutes * 60,
            RefreshToken: refreshToken,
            Scope: authCode.Scopes,
            IdToken: idToken
        );
    }

    private string GenerateAccessToken(User user, Client client, string scopes)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("userId", user.Id.ToString()),
            new("client_id", client.ClientId),
            new("email", user.Email),
            new("name", user.FullName),
            new("preferred_username", user.Username)
        };

        // Add roles
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        // Add permissions
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.Permissions)
            .Where(rp => rp.IsActive && rp.Permission.IsActive)
            .Select(rp => rp.Permission.Key)
            .Distinct();

        claims.Add(new Claim("permissions", string.Join(",", permissions)));

        // Add scopes
        claims.Add(new Claim("scope", scopes));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtCredentials:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtCredentials:Issuer"],
            audience: _configuration["JwtCredentials:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(client.AccessTokenLifetimeMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateIdToken(User user, Client client, string scopes, string? nonce)
    {
        var scopeList = scopes.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("aud", client.ClientId),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (!string.IsNullOrEmpty(nonce))
            claims.Add(new Claim("nonce", nonce));

        if (scopeList.Contains("email"))
        {
            claims.Add(new Claim("email", user.Email));
            claims.Add(new Claim("email_verified", "true"));
        }

        if (scopeList.Contains("profile"))
        {
            claims.Add(new Claim("name", user.FullName));
            claims.Add(new Claim("preferred_username", user.Username));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtCredentials:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtCredentials:Issuer"],
            audience: client.ClientId,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(client.AccessTokenLifetimeMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshToken(Guid userId, CancellationToken cancellationToken)
    {
        var refreshTokenDays = Convert.ToInt32(_configuration["JwtCredentials:RefreshTokenExpiryDays"] ?? "7");

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<RefreshToken>().Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken.Token;
    }

    private static bool ValidatePkce(string codeVerifier, string codeChallenge, string? method)
    {
        if (method == "S256")
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(codeVerifier));
            var computed = Convert.ToBase64String(hash)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
            return computed == codeChallenge;
        }
        else // plain
        {
            return codeVerifier == codeChallenge;
        }
    }
}
