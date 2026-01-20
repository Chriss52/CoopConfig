using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Nubeteck.Extensions.Security;

public class JwtTokenGenerator
{
    private readonly IConfiguration configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public LoginResponse Generate(UserInfo user)
    {
        var jwtConfig = configuration.GetJWTConfiguration();
        LoginResponse response = new LoginResponse()
        {
            ExpiryDate = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtConfig.ExpiryMinutes)),
            Token = String.Empty
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("userId", user.UserId),
            new Claim("email", user.Email!),
            new Claim("userName", user.Username),
            new Claim("name", user.FullName),
            new Claim("expiration", response.ExpiryDate.ToLongDateString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        claims.Add(new Claim("permissions", String.Join(",", user.Permissions.ToArray())));

        var token = new JwtSecurityToken(
            issuer: jwtConfig.Issuer,
            audience: jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        response.Token = new JwtSecurityTokenHandler().WriteToken(token);
        return response;
    }
}
