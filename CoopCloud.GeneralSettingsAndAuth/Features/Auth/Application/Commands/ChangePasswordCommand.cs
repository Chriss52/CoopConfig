using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Security;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : IRequest<bool>;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
{
    private readonly AppDbContext _context;
    private readonly JwtUtils _jwtUtils;

    public ChangePasswordCommandHandler(AppDbContext context, JwtUtils jwtUtils)
    {
        _context = context;
        _jwtUtils = jwtUtils;
    }

    public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Id == command.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new InvalidOperationException("Usuario no encontrado");

        var currentPasswordHash = _jwtUtils.EncryptToSHA256(command.Request.CurrentPassword);

        if (user.PasswordHash != currentPasswordHash)
            throw new UnauthorizedAccessException("La contraseña actual es incorrecta");

        user.PasswordHash = _jwtUtils.EncryptToSHA256(command.Request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        // Revoke all refresh tokens for security
        var userTokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in userTokens)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = "Cambio de contraseña";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
