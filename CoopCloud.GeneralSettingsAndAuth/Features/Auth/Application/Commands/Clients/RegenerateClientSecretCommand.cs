using System.Security.Cryptography;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.Clients;

public record RegenerateClientSecretCommand(Guid ClientId) : IRequest<RegenerateClientSecretResponse>;

public class RegenerateClientSecretCommandHandler : IRequestHandler<RegenerateClientSecretCommand, RegenerateClientSecretResponse>
{
    private readonly AppDbContext _context;

    public RegenerateClientSecretCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RegenerateClientSecretResponse> Handle(RegenerateClientSecretCommand command, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .FirstOrDefaultAsync(c => c.Id == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            throw new InvalidOperationException("Cliente no encontrado");

        if (client.ClientType != "confidential")
            throw new InvalidOperationException("Solo los clientes confidenciales tienen client secret");

        // Generate new secret
        var newSecret = GenerateClientSecret();
        client.ClientSecret = newSecret;
        client.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new RegenerateClientSecretResponse(client.ClientId, newSecret);
    }

    private static string GenerateClientSecret()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
