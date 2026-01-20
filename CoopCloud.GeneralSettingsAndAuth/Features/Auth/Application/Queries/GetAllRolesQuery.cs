using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetAllRolesQuery : IRequest<IEnumerable<RoleDto>>;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleDto>>
{
    private readonly AppDbContext _context;

    public GetAllRolesQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoleDto>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        var roles = await _context.Set<Role>()
            .Include(r => r.UserRoles)
            .Include(r => r.Permissions)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);

        return roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.CreatedAt,
            r.UserRoles.Count,
            r.Permissions.Count
        ));
    }
}
