using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record LogoutCommand(string RefreshToken) : IRequest<bool>;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly AppDbContext _context;

    public LogoutCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken && !rt.IsRevoked, cancellationToken);

        if (refreshToken == null)
            return false;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        refreshToken.RevokedReason = "Logout por usuario";

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
