using System.Security.Cryptography;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Security;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nubeteck.Extensions.Security;
using LoginResponse = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs.LoginResponse;
using PermissionDto = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Security.PermissionDto;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<LoginResponse>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly AppDbContext _context;
    private readonly JwtUtils _jwtUtils;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(AppDbContext context, JwtUtils jwtUtils, IConfiguration configuration)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _context.Set<RefreshToken>()
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.Permissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == command.Request.RefreshToken, cancellationToken);

        if (existingToken == null)
            throw new UnauthorizedAccessException("Token de actualización inválido");

        if (existingToken.IsRevoked)
        {
            // Possible token reuse attack - revoke all descendant tokens
            await RevokeDescendantTokens(existingToken, "Se detectó reutilización de token revocado", cancellationToken);
            throw new UnauthorizedAccessException("Token de actualización ha sido revocado");
        }

        if (existingToken.IsExpired)
            throw new UnauthorizedAccessException("Token de actualización ha expirado");

        var user = existingToken.User;

        if (!user.IsActive || user.IsDeleted)
            throw new UnauthorizedAccessException("La cuenta de usuario no está activa");

        // Rotate refresh token
        var newRefreshToken = RotateRefreshToken(existingToken);
        _context.Set<RefreshToken>().Add(newRefreshToken);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate new JWT token
        var userLoginDto = MapToUserLoginDto(user);
        var token = _jwtUtils.GenerateJwtToken(userLoginDto);
        var expiryMinutes = Convert.ToDouble(_configuration["JwtCredentials:ExpiryMinutes"] ?? "1440");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        return new LoginResponse(
            token,
            newRefreshToken.Token,
            expiresAt,
            new UserDto(
                user.Id,
                user.Email,
                user.Username,
                user.FullName,
                user.PhoneNumber,
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt,
                roles
            )
        );
    }

    private RefreshToken RotateRefreshToken(RefreshToken oldToken)
    {
        var refreshTokenDays = Convert.ToInt32(_configuration["JwtCredentials:RefreshTokenExpiryDays"] ?? "7");

        var newToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = oldToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };

        // Revoke old token
        oldToken.IsRevoked = true;
        oldToken.RevokedAt = DateTime.UtcNow;
        oldToken.ReplacedByToken = newToken.Token;
        oldToken.RevokedReason = "Reemplazado por nuevo token";

        return newToken;
    }

    private async Task RevokeDescendantTokens(RefreshToken token, string reason, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(token.ReplacedByToken))
        {
            var childToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.Token == token.ReplacedByToken, cancellationToken);

            if (childToken != null)
            {
                if (!childToken.IsRevoked)
                {
                    childToken.IsRevoked = true;
                    childToken.RevokedAt = DateTime.UtcNow;
                    childToken.RevokedReason = reason;
                }

                await RevokeDescendantTokens(childToken, reason, cancellationToken);
            }
        }
    }

    private UserLoginDto MapToUserLoginDto(User user)
    {
        return new UserLoginDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            UsersRoles = user.UserRoles.Select(ur => new UserRoleDto
            {
                Rol = new RolDto { RolName = ur.Role.Name },
                Permisos = ur.Role.Permissions
                    .Where(rp => rp.IsActive && rp.Permission.IsActive)
                    .Select(rp => new PermissionDto { PermisoId = rp.Permission.Id })
                    .ToList()
            }).ToList()
        };
    }
}
