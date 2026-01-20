using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record AssignRolesToUserCommand(Guid UserId, AssignRolesToUserRequest Request) : IRequest<UserDto>;

public class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, UserDto>
{
    private readonly AppDbContext _context;

    public AssignRolesToUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(AssignRolesToUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == command.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new InvalidOperationException("Usuario no encontrado");

        // Remove existing roles
        _context.Set<UserRole>().RemoveRange(user.UserRoles);

        // Add new roles
        foreach (var roleId in command.Request.RoleIds)
        {
            var roleExists = await _context.Set<Role>()
                .AnyAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);

            if (!roleExists)
                throw new InvalidOperationException($"Rol con ID {roleId} no encontrado");

            _context.Set<UserRole>().Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Reload roles
        var roles = await _context.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            user.PhoneNumber,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt,
            roles
        );
    }
}
