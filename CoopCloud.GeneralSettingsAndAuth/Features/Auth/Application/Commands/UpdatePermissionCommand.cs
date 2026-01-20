using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record UpdatePermissionCommand(Guid PermissionId, UpdatePermissionRequest Request) : IRequest<PermissionDto>;

public class UpdatePermissionCommandHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
{
    private readonly AppDbContext _context;

    public UpdatePermissionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PermissionDto> Handle(UpdatePermissionCommand command, CancellationToken cancellationToken)
    {
        var permission = await _context.Set<Permission>()
            .FirstOrDefaultAsync(p => p.Id == command.PermissionId && !p.IsDeleted, cancellationToken);

        if (permission == null)
            throw new InvalidOperationException("Permiso no encontrado");

        var request = command.Request;

        if (!string.IsNullOrEmpty(request.Name))
            permission.Name = request.Name;

        if (!string.IsNullOrEmpty(request.Key) && request.Key != permission.Key)
        {
            var keyExists = await _context.Set<Permission>()
                .AnyAsync(p => p.Key == request.Key && p.Id != permission.Id && !p.IsDeleted, cancellationToken);
            if (keyExists)
                throw new InvalidOperationException("Ya existe un permiso con esa clave");
            permission.Key = request.Key;
        }

        if (request.IsActive.HasValue)
            permission.IsActive = request.IsActive.Value;

        permission.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new PermissionDto(
            permission.Id,
            permission.Name,
            permission.Key,
            permission.IsActive,
            permission.CreatedAt
        );
    }
}
