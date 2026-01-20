using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;

public record AddRedirectUriCommand(Guid ClientId, AddRedirectUriRequest Request) : IRequest<ClientRedirectUriDto>;

public class AddRedirectUriCommandHandler : IRequestHandler<AddRedirectUriCommand, ClientRedirectUriDto>
{
    private readonly AppDbContext _context;

    public AddRedirectUriCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClientRedirectUriDto> Handle(AddRedirectUriCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            throw new InvalidOperationException("Cliente no encontrado");

        // Check for duplicate
        var exists = client.RedirectUris.Any(r => r.Uri == command.Request.Uri && !r.IsDeleted);
        if (exists)
            throw new InvalidOperationException("Esta redirect URI ya existe para el cliente");

        // If this is set as default, unset other defaults
        if (command.Request.IsDefault)
        {
            foreach (var uri in client.RedirectUris.Where(r => r.IsDefault && !r.IsDeleted))
            {
                uri.IsDefault = false;
            }
        }

        var redirectUri = new ClientRedirectUri
        {
            Id = Guid.NewGuid(),
            ClientId = client.Id,
            Uri = command.Request.Uri,
            IsDefault = command.Request.IsDefault,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<ClientRedirectUri>().Add(redirectUri);
        await _context.SaveChangesAsync(cancellationToken);

        return new ClientRedirectUriDto(redirectUri.Id, redirectUri.Uri, redirectUri.IsDefault);
    }
}

public record RemoveRedirectUriCommand(Guid ClientId, Guid RedirectUriId) : IRequest<bool>;

public class RemoveRedirectUriCommandHandler : IRequestHandler<RemoveRedirectUriCommand, bool>
{
    private readonly AppDbContext _context;

    public RemoveRedirectUriCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RemoveRedirectUriCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            throw new InvalidOperationException("Cliente no encontrado");

        var redirectUri = client.RedirectUris.FirstOrDefault(r => r.Id == command.RedirectUriId && !r.IsDeleted);
        if (redirectUri == null)
            return false;

        // Ensure at least one redirect URI remains
        var remainingUris = client.RedirectUris.Count(r => !r.IsDeleted && r.Id != command.RedirectUriId);
        if (remainingUris == 0)
            throw new InvalidOperationException("El cliente debe tener al menos una redirect URI");

        // Soft delete
        redirectUri.IsDeleted = true;
        redirectUri.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
