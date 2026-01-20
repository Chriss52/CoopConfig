using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;

namespace CoopCloud.GeneralSettingsAndAuth.Examples;

/// <summary>
/// Ejemplo de cliente OAuth2 para conectarse al servidor de autenticación.
/// Puedes copiar esta clase a tu proyecto cliente.
/// </summary>
public class OAuthClient
{
    private readonly HttpClient _httpClient;
    private readonly OAuthClientOptions _options;

    public OAuthClient(HttpClient httpClient, OAuthClientOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    /// <summary>
    /// Genera la URL para redirigir al usuario al login.
    /// Usa esto en aplicaciones web para iniciar el flujo de autenticación.
    /// </summary>
    public AuthorizationRequest BuildAuthorizationUrl(string? state = null)
    {
        // Generar PKCE (recomendado para seguridad)
        var codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(codeVerifier);
        state ??= GenerateState();

        var query = HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = _options.ClientId;
        query["redirect_uri"] = _options.RedirectUri;
        query["response_type"] = "code";
        query["scope"] = string.Join(" ", _options.Scopes);
        query["state"] = state;
        query["code_challenge"] = codeChallenge;
        query["code_challenge_method"] = "S256";

        var url = $"{_options.AuthorityUrl}/oauth/authorize?{query}";

        return new AuthorizationRequest(url, state, codeVerifier);
    }

    /// <summary>
    /// Intercambia el código de autorización por tokens.
    /// Llama esto después de que el usuario regrese del login.
    /// </summary>
    public async Task<TokenResponse> ExchangeCodeAsync(string code, string codeVerifier, CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = _options.RedirectUri,
            ["client_id"] = _options.ClientId,
            ["code_verifier"] = codeVerifier
        };

        if (!string.IsNullOrEmpty(_options.ClientSecret))
            request["client_secret"] = _options.ClientSecret;

        var response = await _httpClient.PostAsync(
            $"{_options.AuthorityUrl}/oauth/token",
            new FormUrlEncodedContent(request),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Invalid token response");
    }

    /// <summary>
    /// Renueva el access token usando el refresh token.
    /// </summary>
    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = _options.ClientId
        };

        if (!string.IsNullOrEmpty(_options.ClientSecret))
            request["client_secret"] = _options.ClientSecret;

        var response = await _httpClient.PostAsync(
            $"{_options.AuthorityUrl}/oauth/token",
            new FormUrlEncodedContent(request),
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
            ?? throw new InvalidOperationException("Invalid token response");
    }

    /// <summary>
    /// Obtiene información del usuario autenticado.
    /// </summary>
    public async Task<UserInfo> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_options.AuthorityUrl}/oauth/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserInfo>(cancellationToken)
            ?? throw new InvalidOperationException("Invalid userinfo response");
    }

    /// <summary>
    /// Revoca un token (access o refresh).
    /// </summary>
    public async Task RevokeTokenAsync(string token, string tokenTypeHint = "refresh_token", CancellationToken cancellationToken = default)
    {
        var request = new Dictionary<string, string>
        {
            ["token"] = token,
            ["token_type_hint"] = tokenTypeHint,
            ["client_id"] = _options.ClientId
        };

        if (!string.IsNullOrEmpty(_options.ClientSecret))
            request["client_secret"] = _options.ClientSecret;

        var response = await _httpClient.PostAsync(
            $"{_options.AuthorityUrl}/oauth/revoke",
            new FormUrlEncodedContent(request),
            cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Cierra la sesión del usuario.
    /// </summary>
    public string BuildLogoutUrl(string? idToken = null, string? postLogoutRedirectUri = null)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrEmpty(idToken))
            query["id_token_hint"] = idToken;

        if (!string.IsNullOrEmpty(postLogoutRedirectUri))
            query["post_logout_redirect_uri"] = postLogoutRedirectUri;

        return $"{_options.AuthorityUrl}/oauth/logout?{query}";
    }

    // ==================== PKCE Helpers ====================

    private static string GenerateCodeVerifier()
    {
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        var bytes = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(bytes);
    }

    private static string GenerateState()
    {
        var bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}

// ==================== Configuration ====================

public class OAuthClientOptions
{
    /// <summary>
    /// URL base del servidor de autenticación (ej: "https://auth.tuempresa.com")
    /// </summary>
    public string AuthorityUrl { get; set; } = string.Empty;

    /// <summary>
    /// Client ID registrado en el servidor
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client Secret (solo para clientes confidenciales)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// URI a donde el servidor redirige después del login
    /// </summary>
    public string RedirectUri { get; set; } = string.Empty;

    /// <summary>
    /// Scopes a solicitar (por defecto: openid, profile, email)
    /// </summary>
    public string[] Scopes { get; set; } = ["openid", "profile", "email"];
}

// ==================== Response Models ====================

public record AuthorizationRequest(string Url, string State, string CodeVerifier);

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }

    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("sub")]
    public string Sub { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool? EmailVerified { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; set; }

    [JsonPropertyName("roles")]
    public IEnumerable<string>? Roles { get; set; }

    [JsonPropertyName("permissions")]
    public IEnumerable<string>? Permissions { get; set; }
}
