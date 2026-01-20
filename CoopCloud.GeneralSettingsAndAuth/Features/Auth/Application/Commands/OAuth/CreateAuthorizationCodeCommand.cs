using System.Security.Cryptography;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.OAuth;

public record CreateAuthorizationCodeCommand(
    Guid UserId,
    string ClientId,
    string RedirectUri,
    string? Scope,
    string? State,
    string? Nonce,
    string? CodeChallenge,
    string? CodeChallengeMethod
) : IRequest<AuthorizeResponse>;

public class CreateAuthorizationCodeCommandHandler : IRequestHandler<CreateAuthorizationCodeCommand, AuthorizeResponse>
{
    private readonly AppDbContext _context;

    public CreateAuthorizationCodeCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorizeResponse> Handle(CreateAuthorizationCodeCommand command, CancellationToken cancellationToken)
    {
        // Get the client
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.ClientId == command.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null || !client.IsActive)
            throw new InvalidOperationException("Cliente no encontrado o inactivo");

        // Validate redirect URI
        var isValidRedirectUri = client.RedirectUris.Any(r => r.Uri == command.RedirectUri && !r.IsDeleted);
        if (!isValidRedirectUri)
            throw new InvalidOperationException("Redirect URI no autorizada para este cliente");

        // Validate grant type
        var allowedGrantTypes = client.AllowedGrantTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (!allowedGrantTypes.Contains("authorization_code"))
            throw new InvalidOperationException("El cliente no tiene permitido el flujo authorization_code");

        // Validate PKCE if required
        if (client.RequirePkce && string.IsNullOrEmpty(command.CodeChallenge))
            throw new InvalidOperationException("Este cliente requiere PKCE");

        // Generate authorization code
        var code = GenerateAuthorizationCode();

        var authorizationCode = new AuthorizationCode
        {
            Id = Guid.NewGuid(),
            Code = code,
            ClientId = client.Id,
            UserId = command.UserId,
            RedirectUri = command.RedirectUri,
            Scopes = command.Scope ?? "openid",
            CodeChallenge = command.CodeChallenge,
            CodeChallengeMethod = command.CodeChallengeMethod,
            State = command.State,
            Nonce = command.Nonce,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10), // Authorization codes expire in 10 minutes
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<AuthorizationCode>().Add(authorizationCode);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthorizeResponse(code, command.State);
    }

    private static string GenerateAuthorizationCode()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
