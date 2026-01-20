using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetClientForAuthorizationQuery(
    string ClientId,
    string RedirectUri,
    string ResponseType,
    string? Scope,
    string? CodeChallenge
) : IRequest<LoginPageModel>;

public class GetClientForAuthorizationQueryHandler : IRequestHandler<GetClientForAuthorizationQuery, LoginPageModel>
{
    private readonly AppDbContext _context;

    public GetClientForAuthorizationQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LoginPageModel> Handle(GetClientForAuthorizationQuery query, CancellationToken cancellationToken)
    {
        var client = await _context.Set<Client>()
            .Include(c => c.RedirectUris)
            .FirstOrDefaultAsync(c => c.ClientId == query.ClientId && !c.IsDeleted, cancellationToken);

        if (client == null)
            throw new InvalidOperationException("Cliente no encontrado");

        if (!client.IsActive)
            throw new InvalidOperationException("Cliente inactivo");

        // Validate response_type
        if (query.ResponseType != "code")
            throw new InvalidOperationException("response_type debe ser 'code'");

        // Validate redirect URI
        var isValidRedirectUri = client.RedirectUris.Any(r => r.Uri == query.RedirectUri && !r.IsDeleted);
        if (!isValidRedirectUri)
            throw new InvalidOperationException("redirect_uri no autorizada para este cliente");

        // Validate grant type
        var allowedGrantTypes = client.AllowedGrantTypes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (!allowedGrantTypes.Contains("authorization_code"))
            throw new InvalidOperationException("El cliente no tiene permitido el flujo authorization_code");

        // Validate PKCE if required
        if (client.RequirePkce && string.IsNullOrEmpty(query.CodeChallenge))
            throw new InvalidOperationException("Este cliente requiere PKCE (code_challenge)");

        // Validate scopes
        var requestedScopes = (query.Scope ?? "openid").Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var allowedScopes = client.AllowedScopes.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var invalidScopes = requestedScopes.Except(allowedScopes).ToList();

        if (invalidScopes.Any())
            throw new InvalidOperationException($"Scopes no permitidos: {string.Join(", ", invalidScopes)}");

        return new LoginPageModel(
            ClientId: client.ClientId,
            ClientName: client.Name,
            ClientLogoUrl: client.LogoUrl,
            RedirectUri: query.RedirectUri,
            Scope: query.Scope,
            State: null,
            Nonce: null,
            CodeChallenge: query.CodeChallenge,
            CodeChallengeMethod: null,
            ErrorMessage: null
        );
    }
}
