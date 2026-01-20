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

public record RefreshOAuthTokenCommand(
    string RefreshToken,
    string ClientId,
    string? ClientSecret,
    string? Scope
) : IRequest<TokenResponse>;

public class RefreshOAuthTokenCommandHandler : IRequestHandler<RefreshOAuthTokenCommand, TokenResponse>
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public RefreshOAuthTokenCommandHandler(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenResponse> Handle(RefreshOAuthTokenCommand command, CancellationToken cancellationToken)
    {
        // Find the client
        var client = await _context.Set<Client>()
            .FirstOrDefaultAsync(c => c.ClientId == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null || !client.IsActive)
            throw new UnauthorizedAccessException("Cliente no encontrado o inactivo");

        // Validate client secret for confidential clients
        if (client.ClientType == "confidential")
        {
            if (string.IsNullOrEmpty(command.ClientSecret) || client.ClientSecret != command.ClientSecret)
                throw new UnauthorizedAccessException("Client secret inválido");
        }

        // Validate grant type
        var allowedGrantTypes = client.AllowedGrantTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (!allowedGrantTypes.Contains("refresh_token"))
            throw new UnauthorizedAccessException("El cliente no tiene permitido el flujo refresh_token");

        // Find the refresh token
        var existingToken = await _context.Set<RefreshToken>()
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.Permissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, cancellationToken);

        if (existingToken == null)
            throw new UnauthorizedAccessException("Refresh token inválido");

        if (existingToken.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token ha sido revocado");

        if (existingToken.IsExpired)
            throw new UnauthorizedAccessException("Refresh token ha expirado");

        var user = existingToken.User;

        if (!user.IsActive || user.IsDeleted)
            throw new UnauthorizedAccessException("Usuario no activo");

        // Rotate refresh token
        var newRefreshToken = RotateRefreshToken(existingToken, client.RefreshTokenLifetimeDays);
        _context.Set<RefreshToken>().Add(newRefreshToken);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Determine scopes
        var scopes = command.Scope ?? client.AllowedScopes.Replace(",", " ");

        // Generate new access token
        var accessToken = GenerateAccessToken(user, client, scopes);

        await _context.SaveChangesAsync(cancellationToken);

        return new TokenResponse(
            AccessToken: accessToken,
            TokenType: "Bearer",
            ExpiresIn: client.AccessTokenLifetimeMinutes * 60,
            RefreshToken: newRefreshToken.Token,
            Scope: scopes,
            IdToken: null
        );
    }

    private RefreshToken RotateRefreshToken(RefreshToken oldToken, int expiryDays)
    {
        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = oldToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
            CreatedAt = DateTime.UtcNow
        };

        // Revoke old token
        oldToken.IsRevoked = true;
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByToken = newToken.Token;
        oldToken.RevokedReason = "Reemplazado por nuevo token";

        return newToken;
    }

    private string GenerateAccessToken(User user, Client client, string scopes)
    {
        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
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
        claims.Add(new Claim("scope", scopes));

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
}
