using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record AssignPermissionsToRoleCommand(Guid RoleId, AssignPermissionsToRoleRequest Request) : IRequest<RoleDto>;

public class AssignPermissionsToRoleCommandHandler : IRequestHandler<AssignPermissionsToRoleCommand, RoleDto>
{
    private readonly AppDbContext _context;

    public AssignPermissionsToRoleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(AssignPermissionsToRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.Permissions)
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == command.RoleId && !r.IsDeleted, cancellationToken);

        if (role == null)
            throw new InvalidOperationException("Rol no encontrado");

        // Remove existing permissions
        _context.Set<RolePermission>().RemoveRange(role.Permissions);

        // Add new permissions
        foreach (var permissionId in command.Request.PermissionIds)
        {
            var permissionExists = await _context.Set<Permission>()
                .AnyAsync(p => p.Id == permissionId && !p.IsDeleted, cancellationToken);

            if (!permissionExists)
                throw new InvalidOperationException($"Permiso con ID {permissionId} no encontrado");

            _context.Set<RolePermission>().Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId,
                IsActive = true
            });
        }

        role.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return new RoleDto(
            role.Id,
            role.Name,
            role.CreatedAt,
            role.UserRoles.Count,
            command.Request.PermissionIds.Count()
        );
    }
}
