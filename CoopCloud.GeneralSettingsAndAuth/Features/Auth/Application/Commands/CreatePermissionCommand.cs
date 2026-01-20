using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record CreatePermissionCommand(CreatePermissionRequest Request) : IRequest<PermissionDto>;

public class CreatePermissionCommandHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
{
    private readonly AppDbContext _context;

    public CreatePermissionCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PermissionDto> Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var existingPermission = await _context.Set<Permission>()
            .FirstOrDefaultAsync(p => p.Key == request.Key && !p.IsDeleted, cancellationToken);

        if (existingPermission != null)
            throw new InvalidOperationException("Ya existe un permiso con esa clave");

        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Key = request.Key,
            IsActive = true,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<Permission>().Add(permission);
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
