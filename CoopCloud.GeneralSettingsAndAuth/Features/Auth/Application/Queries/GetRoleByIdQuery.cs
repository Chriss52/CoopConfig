using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetRoleByIdQuery(Guid RoleId) : IRequest<RoleDetailDto?>;

public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDetailDto?>
{
    private readonly AppDbContext _context;

    public GetRoleByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleDetailDto?> Handle(GetRoleByIdQuery query, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .Include(r => r.Permissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == query.RoleId && !r.IsDeleted, cancellationToken);

        if (role == null)
            return null;

        var permissions = role.Permissions
            .Where(rp => rp.IsActive)
            .Select(rp => new PermissionDto(
                rp.Permission.Id,
                rp.Permission.Name,
                rp.Permission.Key,
                rp.Permission.IsActive,
                rp.Permission.CreatedAt
            )).ToList();

        var users = role.UserRoles
            .Where(ur => !ur.User.IsDeleted)
            .Select(ur => new UserDto(
                ur.User.Id,
                ur.User.Email,
                ur.User.Username,
                ur.User.FullName,
                ur.User.PhoneNumber,
                ur.User.IsActive,
                ur.User.LastLoginAt,
                ur.User.CreatedAt,
                Array.Empty<string>()
            )).ToList();

        return new RoleDetailDto(
            role.Id,
            role.Name,
            role.CreatedAt,
            role.UpdatedAt,
            permissions,
            users
        );
    }
}
