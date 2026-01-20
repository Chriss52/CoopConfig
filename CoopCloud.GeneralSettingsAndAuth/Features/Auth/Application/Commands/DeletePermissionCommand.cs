using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record DeletePermissionCommand(Guid PermissionId) : IRequest<bool>;

public class DeletePermissionCommandHandler : IRequestHandler<DeletePermissionCommand, bool>
{
    private readonly AppDbContext _context;

    public DeletePermissionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await _context.Set<Permission>()
            .Include(p => p.Roles)
            .FirstOrDefaultAsync(p => p.Id == command.PermissionId && !p.IsDeleted, cancellationToken);

        if (permission == null)
            return false;

        // Soft delete
        permission.IsDeleted = true;
        permission.DeletedAt = DateTime.UtcNow;
        permission.IsActive = false;

        // Also deactivate all role-permission associations
        foreach (var rolePermission in permission.Roles)
        {
            rolePermission.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
