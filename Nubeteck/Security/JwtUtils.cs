using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Nubeteck.Extensions.Security;

public class JwtUtils
{
    private readonly IConfiguration _configuration;

    public JwtUtils(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string EncryptToSHA256(string secret)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(secret));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public string GenerateJwtToken(dynamic user)
    {
        var userClaims = new List<Claim>()
        {
            new Claim("userId", user.UserId.ToString()),
            new Claim("email", user.Email!),
            new Claim("userName", user.Username),
            new Claim("name", user.FullName),
            new Claim("expiration", DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtCredentials:ExpiryMinutes"]!)).ToLongDateString())
        };

        if (user.UsersRoles != null && user.UsersRoles is IEnumerable<dynamic>)
        {
            var userRoles = (IEnumerable<dynamic>)user.UsersRoles;
            List<int> permisos = new();
            foreach (var userRole in userRoles)
            {
                if (userRole.Permisos is IEnumerable<dynamic> permiso)
                {
                    var ids = permiso.Select(p => (int)p.PermisoId).ToList();
                    permisos.AddRange(ids);
                }
            }

            userClaims.Add(new Claim("permisos", string.Join(",", permisos)));

            userClaims.AddRange(userRoles
                .Where(x => x?.Rol?.RolName != null)
                .Select(x => new Claim(ClaimTypes.Role, x.Rol.RolName))
            );
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtCredentials:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        var jwtConfig = new JwtSecurityToken(
            issuer: _configuration["JwtCredentials:Issuer"]!,
            audience: _configuration["JwtCredentials:Audience"]!,
            claims: userClaims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtCredentials:ExpiryMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
    }
}
