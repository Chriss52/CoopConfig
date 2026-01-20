using System.Security.Cryptography;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;

public record CreateClientCommand(CreateClientRequest Request) : IRequest<ClientDetailDto>;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientDetailDto>
{
    private readonly AppDbContext _context;

    public CreateClientCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ClientDetailDto> Handle(CreateClientCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        // Validate at least one redirect URI
        if (request.RedirectUris == null || !request.RedirectUris.Any())
            throw new InvalidOperationException("Debe especificar al menos una redirect URI");

        // Generate unique client ID
        var clientId = GenerateClientId();

        // Generate client secret for confidential clients
        string? clientSecret = null;
        if (request.ClientType == "confidential")
        {
            clientSecret = GenerateClientSecret();
        }

        var client = new Client
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ClientId = clientId,
            ClientSecret = clientSecret,
            ClientType = request.ClientType,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            HomepageUrl = request.HomepageUrl,
            IsActive = true,
            AccessTokenLifetimeMinutes = request.AccessTokenLifetimeMinutes ?? 60,
            RefreshTokenLifetimeDays = request.RefreshTokenLifetimeDays ?? 7,
            RequirePkce = request.RequirePkce,
            AllowedGrantTypes = request.AllowedGrantTypes != null
                ? string.Join(",", request.AllowedGrantTypes)
                : "authorization_code,refresh_token",
            AllowedScopes = request.AllowedScopes != null
                ? string.Join(",", request.AllowedScopes)
                : "openid,profile,email",
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<Client>().Add(client);

        // Add redirect URIs
        var isFirst = true;
        foreach (var uri in request.RedirectUris)
        {
            _context.Set<ClientRedirectUri>().Add(new ClientRedirectUri
            {
                Id = Guid.NewGuid(),
                ClientId = client.Id,
                Uri = uri,
                IsDefault = isFirst,
                CreatedAt = DateTime.UtcNow
            });
            isFirst = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new ClientDetailDto(
            client.Id,
            client.Name,
            client.ClientId,
            clientSecret, // Only returned once!
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
            request.RedirectUris.Select((uri, index) => new ClientRedirectUriDto(
                Guid.Empty, uri, index == 0
            )),
            client.CreatedAt,
            null
        );
    }

    private static string GenerateClientId()
    {
        // Generate a readable client ID like: cc_aBcDeFgHiJkL
        var bytes = RandomNumberGenerator.GetBytes(9);
        return "cc_" + Convert.ToBase64String(bytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, 12);
    }

    private static string GenerateClientSecret()
    {
        // Generate a secure client secret
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
