using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands.OAuth;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nubeteck.Extensions.Security;
using Nubeteck.Extensions.Web;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.API;

public class OAuthSlice : ISlice
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Services are registered elsewhere
    }

    public void RegisterRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/oauth")
            .WithTags("OAuth2 / OpenID Connect");

        // GET /oauth/authorize - Authorization endpoint (redirects to login page)
        group.MapGet("/authorize", (
            string client_id,
            string response_type,
            string redirect_uri,
            string? scope,
            string? state,
            string? nonce,
            string? code_challenge,
            string? code_challenge_method,
            HttpContext context) =>
        {
            // Redirect to login page with all parameters
            var loginUrl = $"/oauth/login.html?client_id={Uri.EscapeDataString(client_id)}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirect_uri)}" +
                          $"&response_type={Uri.EscapeDataString(response_type)}" +
                          $"&scope={Uri.EscapeDataString(scope ?? "openid")}";

            if (!string.IsNullOrEmpty(state))
                loginUrl += $"&state={Uri.EscapeDataString(state)}";
            if (!string.IsNullOrEmpty(nonce))
                loginUrl += $"&nonce={Uri.EscapeDataString(nonce)}";
            if (!string.IsNullOrEmpty(code_challenge))
                loginUrl += $"&code_challenge={Uri.EscapeDataString(code_challenge)}";
            if (!string.IsNullOrEmpty(code_challenge_method))
                loginUrl += $"&code_challenge_method={Uri.EscapeDataString(code_challenge_method)}";

            return Results.Redirect(loginUrl);
        })
        .WithName("OAuthAuthorize")
        .WithDescription("OAuth2 Authorization Endpoint - Redirects to login page")
        .Produces(StatusCodes.Status302Found);

        // GET /oauth/client-info - Get client info for login page (AJAX)
        group.MapGet("/client-info", async (
            string client_id,
            string redirect_uri,
            string response_type,
            string? scope,
            string? code_challenge,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var result = await mediator.Send(new GetClientForAuthorizationQuery(
                    client_id, redirect_uri, response_type, scope, code_challenge), cancellationToken);
                return Results.Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
        })
        .WithName("OAuthClientInfo")
        .WithDescription("Get client info for login page")
        .Produces<LoginPageModel>()
        .Produces(StatusCodes.Status400BadRequest);

        // POST /oauth/login - Process login form submission
        group.MapPost("/login", async (
            LoginFormRequest request,
            IMediator mediator,
            AppDbContext context,
            JwtUtils jwtUtils,
            CancellationToken cancellationToken) =>
        {
            try
            {
                // Validate credentials
                var hashedPassword = jwtUtils.EncryptToSHA256(request.Password);
                var user = await context.Set<User>()
                    .FirstOrDefaultAsync(u => u.Email == request.Email &&
                                             u.PasswordHash == hashedPassword &&
                                             !u.IsDeleted,
                                        cancellationToken);

                if (user == null)
                    return Results.BadRequest(new { error = "El correo y/o contraseña especificados son incorrectos" });

                if (!user.IsActive)
                    return Results.BadRequest(new { error = "La cuenta de usuario está desactivada" });

                // Create authorization code
                var result = await mediator.Send(new CreateAuthorizationCodeCommand(
                    user.Id,
                    request.ClientId,
                    request.RedirectUri,
                    request.Scope,
                    request.State,
                    request.Nonce,
                    request.CodeChallenge,
                    request.CodeChallengeMethod
                ), cancellationToken);

                return Results.Ok(new { code = result.Code, state = result.State });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
        })
        .WithName("OAuthLogin")
        .WithDescription("Process OAuth login form")
        .Produces<AuthorizeResponse>()
        .Produces(StatusCodes.Status400BadRequest);

        // POST /oauth/token - Token endpoint
        group.MapPost("/token", async (
            HttpRequest httpRequest,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            // Support both JSON and form-urlencoded
            string? grantType, code, redirectUri, clientId, clientSecret, refreshToken, codeVerifier, scope;

            if (httpRequest.ContentType?.Contains("application/json") == true)
            {
                var request = await httpRequest.ReadFromJsonAsync<TokenRequest>(cancellationToken);
                grantType = request?.GrantType;
                code = request?.Code;
                redirectUri = request?.RedirectUri;
                clientId = request?.ClientId;
                clientSecret = request?.ClientSecret;
                refreshToken = request?.RefreshToken;
                codeVerifier = request?.CodeVerifier;
                scope = request?.Scope;
            }
            else
            {
                var form = await httpRequest.ReadFormAsync(cancellationToken);
                grantType = form["grant_type"];
                code = form["code"];
                redirectUri = form["redirect_uri"];
                clientId = form["client_id"];
                clientSecret = form["client_secret"];
                refreshToken = form["refresh_token"];
                codeVerifier = form["code_verifier"];
                scope = form["scope"];
            }

            // Check for client credentials in Authorization header
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                var authHeader = httpRequest.Headers.Authorization.FirstOrDefault();
                if (authHeader?.StartsWith("Basic ") == true)
                {
                    var credentials = System.Text.Encoding.UTF8.GetString(
                        Convert.FromBase64String(authHeader.Substring(6))).Split(':');
                    clientId = credentials[0];
                    clientSecret = credentials.Length > 1 ? credentials[1] : null;
                }
            }

            try
            {
                TokenResponse result;

                if (grantType == "authorization_code")
                {
                    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(redirectUri) || string.IsNullOrEmpty(clientId))
                        return Results.BadRequest(new TokenErrorResponse("invalid_request", "Missing required parameters"));

                    result = await mediator.Send(new ExchangeAuthorizationCodeCommand(
                        code, redirectUri, clientId, clientSecret, codeVerifier), cancellationToken);
                }
                else if (grantType == "refresh_token")
                {
                    if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(clientId))
                        return Results.BadRequest(new TokenErrorResponse("invalid_request", "Missing required parameters"));

                    result = await mediator.Send(new RefreshOAuthTokenCommand(
                        refreshToken, clientId, clientSecret, scope), cancellationToken);
                }
                else
                {
                    return Results.BadRequest(new TokenErrorResponse("unsupported_grant_type", "Grant type not supported"));
                }

                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.BadRequest(new TokenErrorResponse("invalid_grant", ex.Message));
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new TokenErrorResponse("server_error", ex.Message));
            }
        })
        .WithName("OAuthToken")
        .WithDescription("OAuth2 Token Endpoint")
        .Produces<TokenResponse>()
        .Produces<TokenErrorResponse>(StatusCodes.Status400BadRequest);

        // GET /oauth/userinfo - UserInfo endpoint (OpenID Connect)
        group.MapGet("/userinfo", async (
            HttpContext context,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Results.Unauthorized();

            var token = authHeader.Substring(7);

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                var scope = jwtToken.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;

                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                    return Results.Unauthorized();

                var result = await mediator.Send(new GetUserInfoQuery(userGuid, scope), cancellationToken);

                if (result == null)
                    return Results.NotFound();

                return Results.Ok(result);
            }
            catch
            {
                return Results.Unauthorized();
            }
        })
        .WithName("OAuthUserInfo")
        .WithDescription("OpenID Connect UserInfo Endpoint")
        .Produces<UserInfoResponse>()
        .Produces(StatusCodes.Status401Unauthorized);

        // POST /oauth/revoke - Token Revocation (RFC 7009)
        group.MapPost("/revoke", async (
            HttpRequest httpRequest,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var (token, tokenTypeHint, clientId, clientSecret) = await ParseTokenRequest(httpRequest, cancellationToken);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(clientId))
                return Results.BadRequest(new { error = "invalid_request", error_description = "Missing required parameters" });

            try
            {
                await mediator.Send(new RevokeTokenCommand(token, tokenTypeHint, clientId, clientSecret), cancellationToken);
                return Results.Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.BadRequest(new { error = "invalid_client", error_description = ex.Message });
            }
        })
        .WithName("OAuthRevoke")
        .WithDescription("RFC 7009 Token Revocation Endpoint")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // POST /oauth/introspect - Token Introspection (RFC 7662)
        group.MapPost("/introspect", async (
            HttpRequest httpRequest,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var (token, tokenTypeHint, clientId, clientSecret) = await ParseTokenRequest(httpRequest, cancellationToken);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(clientId))
                return Results.BadRequest(new { error = "invalid_request", error_description = "Missing required parameters" });

            try
            {
                var result = await mediator.Send(new IntrospectTokenCommand(token, tokenTypeHint, clientId, clientSecret), cancellationToken);
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.BadRequest(new { error = "invalid_client", error_description = ex.Message });
            }
        })
        .WithName("OAuthIntrospect")
        .WithDescription("RFC 7662 Token Introspection Endpoint")
        .Produces<TokenIntrospectionResponse>()
        .Produces(StatusCodes.Status400BadRequest);

        // GET /oauth/logout (End Session) - OpenID Connect RP-Initiated Logout
        group.MapGet("/logout", async (
            string? id_token_hint,
            string? post_logout_redirect_uri,
            string? state,
            AppDbContext context,
            IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            Guid? userId = null;

            // Try to extract user from id_token_hint
            if (!string.IsNullOrEmpty(id_token_hint))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(id_token_hint);
                    var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    if (Guid.TryParse(sub, out var parsedUserId))
                        userId = parsedUserId;
                }
                catch
                {
                    // Invalid token, continue without user context
                }
            }

            // Revoke all active refresh tokens for the user
            if (userId.HasValue)
            {
                var activeTokens = await context.Set<RefreshToken>()
                    .Where(rt => rt.UserId == userId.Value && !rt.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                    token.RevokedReason = "Logout via /oauth/logout";
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            // Redirect to post_logout_redirect_uri if provided
            if (!string.IsNullOrEmpty(post_logout_redirect_uri))
            {
                var redirectUrl = post_logout_redirect_uri;
                if (!string.IsNullOrEmpty(state))
                    redirectUrl += (redirectUrl.Contains('?') ? "&" : "?") + $"state={Uri.EscapeDataString(state)}";
                return Results.Redirect(redirectUrl);
            }

            return Results.Ok(new { message = "Sesión cerrada exitosamente" });
        })
        .WithName("OAuthLogout")
        .WithDescription("OpenID Connect End Session Endpoint")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status302Found);

        // POST /oauth/logout - Alternative POST method for logout
        group.MapPost("/logout", async (
            EndSessionRequest? request,
            AppDbContext context,
            CancellationToken cancellationToken) =>
        {
            Guid? userId = null;

            if (!string.IsNullOrEmpty(request?.IdTokenHint))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(request.IdTokenHint);
                    var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                    if (Guid.TryParse(sub, out var parsedUserId))
                        userId = parsedUserId;
                }
                catch
                {
                    // Invalid token
                }
            }

            if (userId.HasValue)
            {
                var activeTokens = await context.Set<RefreshToken>()
                    .Where(rt => rt.UserId == userId.Value && !rt.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                    token.RevokedAt = DateTime.UtcNow;
                    token.RevokedReason = "Logout via /oauth/logout POST";
                }

                await context.SaveChangesAsync(cancellationToken);
            }

            if (!string.IsNullOrEmpty(request?.PostLogoutRedirectUri))
            {
                var redirectUrl = request.PostLogoutRedirectUri;
                if (!string.IsNullOrEmpty(request.State))
                    redirectUrl += (redirectUrl.Contains('?') ? "&" : "?") + $"state={Uri.EscapeDataString(request.State)}";
                return Results.Ok(new EndSessionResponse(true, redirectUrl));
            }

            return Results.Ok(new EndSessionResponse(true, null));
        })
        .WithName("OAuthLogoutPost")
        .WithDescription("OpenID Connect End Session Endpoint (POST)")
        .Produces<EndSessionResponse>();

        // GET /.well-known/openid-configuration - OpenID Connect Discovery
        app.MapGet("/.well-known/openid-configuration", (HttpContext context, IConfiguration configuration) =>
        {
            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
            var issuer = configuration["JwtCredentials:Issuer"] ?? baseUrl;

            return Results.Ok(new OpenIdConfiguration(
                Issuer: issuer,
                AuthorizationEndpoint: $"{baseUrl}/oauth/authorize",
                TokenEndpoint: $"{baseUrl}/oauth/token",
                UserinfoEndpoint: $"{baseUrl}/oauth/userinfo",
                JwksUri: $"{baseUrl}/.well-known/jwks.json",
                RevocationEndpoint: $"{baseUrl}/oauth/revoke",
                IntrospectionEndpoint: $"{baseUrl}/oauth/introspect",
                EndSessionEndpoint: $"{baseUrl}/oauth/logout",
                ResponseTypesSupported: new[] { "code" },
                SubjectTypesSupported: new[] { "public" },
                IdTokenSigningAlgValuesSupported: new[] { "HS256" },
                ScopesSupported: new[] { "openid", "profile", "email", "roles", "permissions" },
                TokenEndpointAuthMethodsSupported: new[] { "client_secret_basic", "client_secret_post" },
                ClaimsSupported: new[] { "sub", "email", "email_verified", "name", "preferred_username", "roles", "permissions" },
                CodeChallengeMethodsSupported: new[] { "S256", "plain" },
                GrantTypesSupported: new[] { "authorization_code", "refresh_token" }
            ));
        })
        .WithName("OpenIdConfiguration")
        .WithTags("OAuth2 / OpenID Connect")
        .WithDescription("OpenID Connect Discovery Document")
        .Produces<OpenIdConfiguration>();

        // GET /.well-known/jwks.json - JSON Web Key Set
        app.MapGet("/.well-known/jwks.json", (IConfiguration configuration) =>
        {
            // For HS256 (symmetric), we don't expose the key publicly
            // This endpoint is mainly for RS256 (asymmetric) keys
            // We return an empty key set since we use symmetric signing
            var keyId = ComputeKeyId(configuration["JwtCredentials:Key"]!);

            return Results.Ok(new JwksResponse(
                Keys: new[]
                {
                    new JsonWebKeyDto(
                        Kty: "oct",
                        Use: "sig",
                        Kid: keyId,
                        Alg: "HS256",
                        N: null,
                        E: null
                    )
                }
            ));
        })
        .WithName("Jwks")
        .WithTags("OAuth2 / OpenID Connect")
        .WithDescription("JSON Web Key Set - Public keys for token verification")
        .Produces<JwksResponse>();
    }

    /// <summary>
    /// Parse token request from either JSON or form-urlencoded body
    /// </summary>
    private static async Task<(string? Token, string? TokenTypeHint, string? ClientId, string? ClientSecret)> ParseTokenRequest(
        HttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string? token, tokenTypeHint, clientId, clientSecret;

        if (httpRequest.ContentType?.Contains("application/json") == true)
        {
            var request = await httpRequest.ReadFromJsonAsync<IntrospectTokenRequest>(cancellationToken);
            token = request?.Token;
            tokenTypeHint = request?.TokenTypeHint;
            clientId = request?.ClientId;
            clientSecret = request?.ClientSecret;
        }
        else
        {
            var form = await httpRequest.ReadFormAsync(cancellationToken);
            token = form["token"];
            tokenTypeHint = form["token_type_hint"];
            clientId = form["client_id"];
            clientSecret = form["client_secret"];
        }

        // Check for client credentials in Authorization header
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            var authHeader = httpRequest.Headers.Authorization.FirstOrDefault();
            if (authHeader?.StartsWith("Basic ") == true)
            {
                var credentials = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authHeader.Substring(6))).Split(':');
                clientId = credentials[0];
                clientSecret = credentials.Length > 1 ? credentials[1] : null;
            }
        }

        return (token, tokenTypeHint, clientId, clientSecret);
    }

    /// <summary>
    /// Compute a stable key ID from the secret key
    /// </summary>
    private static string ComputeKeyId(string key)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(key));
        return Convert.ToBase64String(hash[..8]).TrimEnd('=');
    }
}
