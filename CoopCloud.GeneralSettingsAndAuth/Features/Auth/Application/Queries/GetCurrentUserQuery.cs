using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetCurrentUserQuery(Guid UserId) : IRequest<UserDetailDto?>;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDetailDto?>
{
    private readonly AppDbContext _context;

    public GetCurrentUserQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetailDto?> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == query.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            return null;

        var roles = user.UserRoles.Select(ur => new RoleDto(
            ur.Role.Id,
            ur.Role.Name,
            ur.Role.CreatedAt,
            0,
            ur.Role.Permissions.Count
        )).ToList();

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.Permissions)
            .Where(rp => rp.IsActive && rp.Permission.IsActive)
            .Select(rp => rp.Permission.Key)
            .Distinct()
            .ToList();

        return new UserDetailDto(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            user.PhoneNumber,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt,
            user.UpdatedAt,
            roles,
            permissions
        );
    }
}
