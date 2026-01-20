using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.OAuth;

/// <summary>
/// RFC 7662 - Token Introspection endpoint
/// Allows resource servers to query the authorization server about the state of a token
/// </summary>
public record IntrospectTokenCommand(
    string Token,
    string? TokenTypeHint,  // "access_token" or "refresh_token"
    string ClientId,
    string? ClientSecret
) : IRequest<TokenIntrospectionResponse>;

public class IntrospectTokenCommandHandler : IRequestHandler<IntrospectTokenCommand, TokenIntrospectionResponse>
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public IntrospectTokenCommandHandler(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenIntrospectionResponse> Handle(IntrospectTokenCommand command, CancellationToken cancellationToken)
    {
        // Validate client
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

        // Try to introspect as refresh token
        if (command.TokenTypeHint == "refresh_token" || string.IsNullOrEmpty(command.TokenTypeHint))
        {
            var refreshToken = await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == command.Token, cancellationToken);

            if (refreshToken != null)
            {
                return new TokenIntrospectionResponse(
                    Active: refreshToken.IsActive,
                    Scope: null,
                    ClientId: command.ClientId,
                    Username: refreshToken.User?.Username,
                    TokenType: "refresh_token",
                    Exp: new DateTimeOffset(refreshToken.ExpiresAt).ToUnixTimeSeconds(),
                    Iat: new DateTimeOffset(refreshToken.CreatedAt).ToUnixTimeSeconds(),
                    Nbf: null,
                    Sub: refreshToken.UserId.ToString(),
                    Aud: null,
                    Iss: _configuration["JwtCredentials:Issuer"],
                    Jti: refreshToken.Id.ToString()
                );
            }
        }

        // Try to introspect as access token (JWT)
        if (command.TokenTypeHint == "access_token" || string.IsNullOrEmpty(command.TokenTypeHint))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtCredentials:Key"]!));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtCredentials:Issuer"],
                    ValidateAudience = false, // We accept tokens for any registered client
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = handler.ValidateToken(command.Token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var sub = principal.FindFirst("sub")?.Value;
                var scope = principal.FindFirst("scope")?.Value;
                var clientIdClaim = principal.FindFirst("client_id")?.Value;

                return new TokenIntrospectionResponse(
                    Active: true,
                    Scope: scope,
                    ClientId: clientIdClaim,
                    Username: principal.FindFirst("preferred_username")?.Value,
                    TokenType: "Bearer",
                    Exp: new DateTimeOffset(jwtToken.ValidTo).ToUnixTimeSeconds(),
                    Iat: new DateTimeOffset(jwtToken.IssuedAt).ToUnixTimeSeconds(),
                    Nbf: jwtToken.Payload.NotBefore,
                    Sub: sub,
                    Aud: jwtToken.Audiences.FirstOrDefault(),
                    Iss: jwtToken.Issuer,
                    Jti: jwtToken.Id
                );
            }
            catch
            {
                // Token is invalid or expired
            }
        }

        // Token not found or invalid - return inactive
        return new TokenIntrospectionResponse(
            Active: false,
            Scope: null,
            ClientId: null,
            Username: null,
            TokenType: null,
            Exp: null,
            Iat: null,
            Nbf: null,
            Sub: null,
            Aud: null,
            Iss: null,
            Jti: null
        );
    }
}
