using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record DeleteRoleCommand(Guid RoleId) : IRequest<bool>;

public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly AppDbContext _context;

    public DeleteRoleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == command.RoleId && !r.IsDeleted, cancellationToken);

        if (role == null)
            return false;

        if (role.UserRoles.Any())
            throw new InvalidOperationException("No se puede eliminar un rol que tiene usuarios asignados");

        // Soft delete
        role.IsDeleted = true;
        role.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
