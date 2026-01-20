using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetAllPermissionsQuery : IRequest<IEnumerable<PermissionDto>>;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, IEnumerable<PermissionDto>>
{
    private readonly AppDbContext _context;

    public GetAllPermissionsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PermissionDto>> Handle(GetAllPermissionsQuery query, CancellationToken cancellationToken)
    {
        var permissions = await _context.Set<Permission>()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Key)
            .ToListAsync(cancellationToken);

        return permissions.Select(p => new PermissionDto(
            p.Id,
            p.Name,
            p.Key,
            p.IsActive,
            p.CreatedAt
        ));
    }
}
