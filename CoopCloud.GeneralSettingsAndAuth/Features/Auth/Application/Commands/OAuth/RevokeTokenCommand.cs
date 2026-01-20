using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.OAuth;

/// <summary>
/// RFC 7009 - Token Revocation endpoint
/// Allows clients to revoke access or refresh tokens
/// </summary>
public record RevokeTokenCommand(
    string Token,
    string? TokenTypeHint,  // "access_token" or "refresh_token"
    string ClientId,
    string? ClientSecret
) : IRequest<bool>;

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, bool>
{
    private readonly AppDbContext _context;

    public RevokeTokenCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
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

        // Try to revoke as refresh token first (most common case)
        var refreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == command.Token && !rt.IsRevoked, cancellationToken);

        if (refreshToken != null)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedReason = "Revocado via /oauth/revoke";
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // For access tokens (JWTs), we can't truly revoke them since they're stateless
        // But we can revoke all refresh tokens for the user if needed
        // Per RFC 7009, we should return success even if the token wasn't found
        // This prevents attackers from using the endpoint to detect valid tokens

        return true;
    }
}
