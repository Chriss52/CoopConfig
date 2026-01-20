using Microsoft.Extensions.Configuration;

namespace Nubeteck.Extensions.Security;

public sealed class JwtConfiguration
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Key { get; set; }
    public int ExpiryMinutes { get; set; }
}

public static class JWTConfigurationExtensions
{
    public static JwtConfiguration GetJWTConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("JwtCredentials").Get<JwtConfiguration>() ??
               configuration.GetSection("Jwt").Get<JwtConfiguration>() ??
               throw new InvalidOperationException("JWT configuration not found");
    }
}
