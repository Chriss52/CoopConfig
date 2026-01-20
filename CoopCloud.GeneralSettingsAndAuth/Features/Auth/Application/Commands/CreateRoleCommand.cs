using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record CreateRoleCommand(CreateRoleRequest Request) : IRequest<RoleDto>;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
{
    private readonly AppDbContext _context;

    public CreateRoleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var existingRole = await _context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == request.Name && !r.IsDeleted, cancellationToken);

        if (existingRole != null)
            throw new InvalidOperationException("Ya existe un rol con ese nombre");

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<Role>().Add(role);

        // Assign permissions if provided
        if (request.PermissionIds != null && request.PermissionIds.Any())
        {
            foreach (var permissionId in request.PermissionIds)
            {
                var permissionExists = await _context.Set<Permission>()
                    .AnyAsync(p => p.Id == permissionId && !p.IsDeleted, cancellationToken);

                if (permissionExists)
                {
                    _context.Set<RolePermission>().Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId,
                        IsActive = true
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var permissionCount = request.PermissionIds?.Count() ?? 0;

        return new RoleDto(
            role.Id,
            role.Name,
            role.CreatedAt,
            0,
            permissionCount
        );
    }
}
