using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetClientByIdQuery(Guid ClientId) : IRequest<ClientDetailDto?>;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientDetailDto?>
{
    private readonly AppDbContext _context;

    public GetClientByIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClientDetailDto?> Handle(GetClientByIdQuery query, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.Id == query.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            return null;

        return new ClientDetailDto(
            client.Id,
            client.Name,
            client.ClientId,
            null, // Never return secret in queries
            client.ClientType,
            client.Description,
            client.LogoUrl,
            client.HomepageUrl,
            client.IsActive,
            client.AccessTokenLifetimeMinutes,
            client.RefreshTokenLifetimeDays,
            client.RequirePkce,
            client.AllowedGrantTypes.Split(',', StringSplitOptions.RemoveEmptyEntries),
            client.AllowedScopes.Split(',', StringSplitOptions.RemoveEmptyEntries),
            client.RedirectUris.Where(r => !r.IsDeleted).Select(r => new ClientRedirectUriDto(
                r.Id, r.Uri, r.IsDefault
            )),
            client.CreatedAt,
            client.UpdatedAt
        );
    }
}
