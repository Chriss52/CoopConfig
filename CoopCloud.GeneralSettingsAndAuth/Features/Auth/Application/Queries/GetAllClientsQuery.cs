using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetAllClientsQuery : IRequest<IEnumerable<ClientDto>>;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IEnumerable<ClientDto>>
{
    private readonly AppDbContext _context;

    public GetAllClientsQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ClientDto>> Handle(GetAllClientsQuery query, CancellationToken cancellationToken)
    {
        var clients = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .Where(c => !c.IsDeleted)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return clients.Select(c => new ClientDto(
            c.Id,
            c.Name,
            c.ClientId,
            c.ClientType,
            c.Description,
            c.LogoUrl,
            c.HomepageUrl,
            c.IsActive,
            c.AccessTokenLifetimeMinutes,
            c.RefreshTokenLifetimeDays,
            c.RequirePkce,
            c.AllowedGrantTypes.Split(',', StringSplitOptions.RemoveEmptyEntries),
            c.AllowedScopes.Split(',', StringSplitOptions.RemoveEmptyEntries),
            c.RedirectUris.Where(r => !r.IsDeleted).Select(r => r.Uri),
            c.CreatedAt
        ));
    }
}
