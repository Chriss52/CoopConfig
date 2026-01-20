using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record UpdateRoleCommand(Guid RoleId, UpdateRoleRequest Request) : IRequest<RoleDto>;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, RoleDto>
{
    private readonly AppDbContext _context;

    public UpdateRoleCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDto> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.UserRoles)
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == command.RoleId && !r.IsDeleted, cancellationToken);

        if (role == null)
            throw new InvalidOperationException("Rol no encontrado");

        var request = command.Request;

        if (!string.IsNullOrEmpty(request.Name) && request.Name != role.Name)
        {
            var nameExists = await _context.Set<Role>()
                .AnyAsync(r => r.Name == request.Name && r.Id != role.Id && !r.IsDeleted, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("Ya existe un rol con ese nombre");
            role.Name = request.Name;
        }

        role.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new RoleDto(
            role.Id,
            role.Name,
            role.CreatedAt,
            role.UserRoles.Count,
            role.Permissions.Count
        );
    }
}
