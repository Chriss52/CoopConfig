namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;

// ==================== OAuth2 Authorization Request ====================

/// <summary>
/// OAuth2 Authorization Request (sent to /oauth/authorize)
/// </summary>
public record AuthorizeRequest(
    string ClientId,
    string ResponseType,       // "code" for Authorization Code flow
    string RedirectUri,
    string? Scope,             // e.g., "openid profile email"
    string? State,             // CSRF protection
    string? Nonce,             // OpenID Connect nonce
    string? CodeChallenge,     // PKCE
    string? CodeChallengeMethod // "S256" or "plain"
);

/// <summary>
/// Response when authorization is successful (redirected to client)
/// </summary>
public record AuthorizeResponse(
    string Code,
    string? State
);

/// <summary>
/// Error response for authorization failures
/// </summary>
public record AuthorizeErrorResponse(
    string Error,
    string? ErrorDescription,
    string? State
);

// ==================== OAuth2 Token Request ====================

/// <summary>
/// OAuth2 Token Request (sent to /oauth/token)
/// </summary>
public record TokenRequest(
    string GrantType,          // "authorization_code" or "refresh_token"
    string? Code,              // Authorization code (for authorization_code grant)
    string? RedirectUri,       // Must match the one used in authorize request
    string? ClientId,          // Required for public clients
    string? ClientSecret,      // Required for confidential clients
    string? RefreshToken,      // For refresh_token grant
    string? CodeVerifier,      // PKCE code verifier
    string? Scope              // Requested scopes (for refresh)
);

/// <summary>
/// OAuth2 Token Response
/// </summary>
public record TokenResponse(
    string AccessToken,
    string TokenType,          // "Bearer"
    int ExpiresIn,             // Seconds until expiration
    string? RefreshToken,
    string? Scope,
    string? IdToken            // OpenID Connect ID Token
);

/// <summary>
/// OAuth2 Token Error Response
/// </summary>
public record TokenErrorResponse(
    string Error,
    string? ErrorDescription
);

// ==================== OpenID Connect UserInfo ====================

/// <summary>
/// OpenID Connect UserInfo Response
/// </summary>
public record UserInfoResponse(
    string Sub,                // Subject (user ID)
    string? Email,
    bool? EmailVerified,
    string? Name,
    string? PreferredUsername,
    string? Picture,
    string? PhoneNumber,
    IEnumerable<string>? Roles,
    IEnumerable<string>? Permissions
);

// ==================== Client Management DTOs ====================

public record ClientDto(
    Guid Id,
    string Name,
    string ClientId,
    string ClientType,
    string? Description,
    string? LogoUrl,
    string? HomepageUrl,
    bool IsActive,
    int AccessTokenLifetimeMinutes,
    int RefreshTokenLifetimeDays,
    bool RequirePkce,
    IEnumerable<string> AllowedGrantTypes,
    IEnumerable<string> AllowedScopes,
    IEnumerable<string> RedirectUris,
    DateTime CreatedAt
);

public record ClientDetailDto(
    Guid Id,
    string Name,
    string ClientId,
    string? ClientSecret,      // Only shown once when created or regenerated
    string ClientType,
    string? Description,
    string? LogoUrl,
    string? HomepageUrl,
    bool IsActive,
    int AccessTokenLifetimeMinutes,
    int RefreshTokenLifetimeDays,
    bool RequirePkce,
    IEnumerable<string> AllowedGrantTypes,
    IEnumerable<string> AllowedScopes,
    IEnumerable<ClientRedirectUriDto> RedirectUris,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record ClientRedirectUriDto(
    Guid Id,
    string Uri,
    bool IsDefault
);

public record CreateClientRequest(
    string Name,
    string? Description,
    string? LogoUrl,
    string? HomepageUrl,
    string ClientType,         // "confidential" or "public"
    bool RequirePkce,
    int? AccessTokenLifetimeMinutes,
    int? RefreshTokenLifetimeDays,
    IEnumerable<string>? AllowedGrantTypes,
    IEnumerable<string>? AllowedScopes,
    IEnumerable<string> RedirectUris
);

public record UpdateClientRequest(
    string? Name,
    string? Description,
    string? LogoUrl,
    string? HomepageUrl,
    bool? IsActive,
    bool? RequirePkce,
    int? AccessTokenLifetimeMinutes,
    int? RefreshTokenLifetimeDays,
    IEnumerable<string>? AllowedGrantTypes,
    IEnumerable<string>? AllowedScopes
);

public record AddRedirectUriRequest(
    string Uri,
    bool IsDefault = false
);

public record RegenerateClientSecretResponse(
    string ClientId,
    string ClientSecret
);

// ==================== Login Page Model ====================

/// <summary>
/// Model for the login page
/// </summary>
public record LoginPageModel(
    string ClientId,
    string ClientName,
    string? ClientLogoUrl,
    string RedirectUri,
    string? Scope,
    string? State,
    string? Nonce,
    string? CodeChallenge,
    string? CodeChallengeMethod,
    string? ErrorMessage
);

/// <summary>
/// Login form submission
/// </summary>
public record LoginFormRequest(
    string Email,
    string Password,
    string ClientId,
    string RedirectUri,
    string? Scope,
    string? State,
    string? Nonce,
    string? CodeChallenge,
    string? CodeChallengeMethod,
    bool RememberMe
);

// ==================== Well-Known Configuration ====================

/// <summary>
/// OpenID Connect Discovery Document
/// </summary>
public record OpenIdConfiguration(
    string Issuer,
    string AuthorizationEndpoint,
    string TokenEndpoint,
    string UserinfoEndpoint,
    string JwksUri,
    string? RevocationEndpoint,
    string? IntrospectionEndpoint,
    string? EndSessionEndpoint,
    IEnumerable<string> ResponseTypesSupported,
    IEnumerable<string> SubjectTypesSupported,
    IEnumerable<string> IdTokenSigningAlgValuesSupported,
    IEnumerable<string> ScopesSupported,
    IEnumerable<string> TokenEndpointAuthMethodsSupported,
    IEnumerable<string> ClaimsSupported,
    IEnumerable<string> CodeChallengeMethodsSupported,
    IEnumerable<string> GrantTypesSupported
);

// ==================== Token Revocation (RFC 7009) ====================

/// <summary>
/// Request to revoke a token
/// </summary>
public record RevokeTokenRequest(
    string Token,
    string? TokenTypeHint,  // "access_token" or "refresh_token"
    string? ClientId,
    string? ClientSecret
);

// ==================== Token Introspection (RFC 7662) ====================

/// <summary>
/// Request to introspect a token
/// </summary>
public record IntrospectTokenRequest(
    string Token,
    string? TokenTypeHint,  // "access_token" or "refresh_token"
    string? ClientId,
    string? ClientSecret
);

/// <summary>
/// RFC 7662 Token Introspection Response
/// </summary>
public record TokenIntrospectionResponse(
    bool Active,
    string? Scope,
    string? ClientId,
    string? Username,
    string? TokenType,
    long? Exp,
    long? Iat,
    long? Nbf,
    string? Sub,
    string? Aud,
    string? Iss,
    string? Jti
);

// ==================== JWKS (JSON Web Key Set) ====================

/// <summary>
/// JSON Web Key Set response
/// </summary>
public record JwksResponse(
    IEnumerable<JsonWebKeyDto> Keys
);

/// <summary>
/// JSON Web Key
/// </summary>
public record JsonWebKeyDto(
    string Kty,      // Key type (e.g., "oct" for symmetric, "RSA" for RSA)
    string Use,      // Public key use (e.g., "sig" for signature)
    string Kid,      // Key ID
    string Alg,      // Algorithm (e.g., "HS256", "RS256")
    string? N,       // RSA modulus (for RSA keys)
    string? E        // RSA exponent (for RSA keys)
);

// ==================== End Session (Logout) ====================

/// <summary>
/// OpenID Connect End Session Request
/// </summary>
public record EndSessionRequest(
    string? IdTokenHint,
    string? PostLogoutRedirectUri,
    string? State
);

/// <summary>
/// Response for end session
/// </summary>
public record EndSessionResponse(
    bool Success,
    string? RedirectUri
);
