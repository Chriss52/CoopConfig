using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Common;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

/// <summary>
/// Represents a registered OAuth2 client application that can request authentication.
/// Similar to how applications are registered in Google Cloud Console or Azure AD.
/// </summary>
public class Client : AuditableEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Human-readable name for the application (e.g., "CoopCloud Inventory System")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Public identifier for the client (sent in authorization requests)
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Secret key for confidential clients (server-side apps)
    /// Null for public clients (SPAs, mobile apps)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Type of client: "confidential" (server-side) or "public" (SPA/mobile)
    /// </summary>
    public string ClientType { get; set; } = "confidential";

    /// <summary>
    /// Description of the application
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// URL to the application's logo (shown on login page)
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// URL to the application's homepage
    /// </summary>
    public string? HomepageUrl { get; set; }

    /// <summary>
    /// Whether this client is active and can request authentication
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Access token lifetime in minutes for this client
    /// </summary>
    public int AccessTokenLifetimeMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token lifetime in days for this client
    /// </summary>
    public int RefreshTokenLifetimeDays { get; set; } = 7;

    /// <summary>
    /// Whether to require PKCE for this client (recommended for public clients)
    /// </summary>
    public bool RequirePkce { get; set; } = false;

    /// <summary>
    /// Allowed OAuth2 grant types (e.g., "authorization_code", "refresh_token")
    /// Stored as comma-separated values
    /// </summary>
    public string AllowedGrantTypes { get; set; } = "authorization_code,refresh_token";

    /// <summary>
    /// Allowed scopes for this client (e.g., "openid,profile,email")
    /// Stored as comma-separated values
    /// </summary>
    public string AllowedScopes { get; set; } = "openid,profile,email";

    public virtual ICollection<ClientRedirectUri> RedirectUris { get; set; } = new List<ClientRedirectUri>();
    public virtual ICollection<AuthorizationCode> AuthorizationCodes { get; set; } = new List<AuthorizationCode>();
}

/// <summary>
/// Authorized redirect URIs for a client application.
/// The authorization server will only redirect to URIs in this list.
/// </summary>
public class ClientRedirectUri : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }

    /// <summary>
    /// The authorized redirect URI (e.g., "https://myapp.com/callback")
    /// </summary>
    public string Uri { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is the default redirect URI
    /// </summary>
    public bool IsDefault { get; set; } = false;

    public virtual Client Client { get; set; } = null!;
}

/// <summary>
/// Short-lived authorization codes used in the OAuth2 Authorization Code flow.
/// These are exchanged for access tokens.
/// </summary>
public class AuthorizationCode : AuditableEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// The authorization code string (sent to client in redirect)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    public Guid ClientId { get; set; }
    public Guid UserId { get; set; }

    /// <summary>
    /// The redirect URI that was used in the authorization request
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Scopes granted in this authorization
    /// </summary>
    public string Scopes { get; set; } = string.Empty;

    /// <summary>
    /// PKCE code challenge (for public clients)
    /// </summary>
    public string? CodeChallenge { get; set; }

    /// <summary>
    /// PKCE code challenge method (usually "S256")
    /// </summary>
    public string? CodeChallengeMethod { get; set; }

    /// <summary>
    /// State parameter from the original request (for CSRF protection)
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Nonce for OpenID Connect
    /// </summary>
    public string? Nonce { get; set; }

    /// <summary>
    /// When this code expires (typically 10 minutes)
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Whether this code has been used (codes are single-use)
    /// </summary>
    public bool IsUsed { get; set; } = false;

    /// <summary>
    /// When the code was used
    /// </summary>
    public DateTime? UsedAt { get; set; }

    public virtual Client Client { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsValid => !IsUsed && !IsExpired;
}
