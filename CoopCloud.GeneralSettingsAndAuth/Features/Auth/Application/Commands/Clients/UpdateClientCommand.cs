using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;

public record UpdateClientCommand(Guid ClientId, UpdateClientRequest Request) : IRequest<ClientDto>;

public class UpdateClientCommandHandler : IRequestHandler<UpdateClientCommand, ClientDto>
{
    private readonly AppDbContext _context;

    public UpdateClientCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClientDto> Handle(UpdateClientCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            throw new InvalidOperationException("Cliente no encontrado");

        var request = command.Request;

        if (!string.IsNullOrEmpty(request.Name))
            client.Name = request.Name;

        if (request.Description != null)
            client.Description = request.Description;

        if (request.LogoUrl != null)
            client.LogoUrl = request.LogoUrl;

        if (request.HomepageUrl != null)
            client.HomepageUrl = request.HomepageUrl;

        if (request.IsActive.HasValue)
            client.IsActive = request.IsActive.Value;

        if (request.RequirePkce.HasValue)
            client.RequirePkce = request.RequirePkce.Value;

        if (request.AccessTokenLifetimeMinutes.HasValue)
            client.AccessTokenLifetimeMinutes = request.AccessTokenLifetimeMinutes.Value;

        if (request.RefreshTokenLifetimeDays.HasValue)
            client.RefreshTokenLifetimeDays = request.RefreshTokenLifetimeDays.Value;

        if (request.AllowedGrantTypes != null)
            client.AllowedGrantTypes = string.Join(",", request.AllowedGrantTypes);

        if (request.AllowedScopes != null)
            client.AllowedScopes = string.Join(",", request.AllowedScopes);

        client.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new ClientDto(
            client.Id,
            client.Name,
            client.ClientId,
            client.ClientType,
            client.Description,
            client.LogoUrl,
            client.HomepageUrl,
            client.IsActive,
            client.AccessTokenLifetimeMinutes,
            client.RefreshTokenLifetimeDays,
            client.RequirePkce,
            client.AllowedGrantTypes.Split(','),
            client.AllowedScopes.Split(','),
            client.RedirectUris.Where(r => !r.IsDeleted).Select(r => r.Uri),
            client.CreatedAt
        );
    }
}
