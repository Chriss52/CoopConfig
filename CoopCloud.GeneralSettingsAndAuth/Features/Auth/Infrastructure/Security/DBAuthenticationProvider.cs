using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Security;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Security;

public class DBAuthenticationProvider : ICredentialProvider
{
    private readonly JwtUtils _jwtUtils;
    private readonly AppDbContext _context;

    public DBAuthenticationProvider(JwtUtils jwtUtils, AppDbContext context)
    {
        _jwtUtils = jwtUtils;
        _context = context;
    }

    public async Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRolesAndPermissionsAsync(email, password, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("El correo y/o contraseña especificados son incorrectos");
        }

        return _jwtUtils.GenerateJwtToken(user);
    }

    public Task<string> LoginActiveDirectoryAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Active Directory authentication not implemented yet");
    }

    public async Task<bool> IsValidAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var hashedPassword = _jwtUtils.EncryptToSHA256(password);
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hashedPassword && !u.IsDeleted, cancellationToken);

        return user != null;
    }

    public async Task<UserInfo> GetUserInfoAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await (from u in _context.Set<User>()
                         where u.Email == email && !u.IsDeleted
                         select new UserInfo
                         {
                             UserId = u.Id.ToString(),
                             Email = u.Email,
                             Username = u.Username,
                             FullName = u.FullName,
                             Roles = (from ur in _context.Set<UserRole>()
                                     join r in _context.Set<Role>() on ur.RoleId equals r.Id
                                     where ur.UserId == u.Id
                                     select r.Name).ToList(),
                             Permissions = (from ur in _context.Set<UserRole>()
                                           join rp in _context.Set<RolePermission>() on ur.RoleId equals rp.RoleId
                                           join p in _context.Set<Permission>() on rp.PermissionId equals p.Id
                                           where ur.UserId == u.Id && rp.IsActive && p.IsActive
                                           select p.Id.ToString()).Distinct().ToList()
                         })
                        .FirstOrDefaultAsync(cancellationToken);

        return user ?? throw new ArgumentException($"User with email {email} not found");
    }

    public bool IsValid(string email, string password)
    {
        return IsValidAsync(email, password).GetAwaiter().GetResult();
    }

    public UserInfo GetUserInfo(string email)
    {
        return GetUserInfoAsync(email).GetAwaiter().GetResult();
    }

    private async Task<UserLoginDto?> GetUserWithRolesAndPermissionsAsync(string email, string password, CancellationToken cancellationToken)
    {
        var hashedPassword = _jwtUtils.EncryptToSHA256(password);

        var user = await _context.Set<User>()
            .Where(u => u.Email == email && u.PasswordHash == hashedPassword && !u.IsDeleted)
            .Select(u => new UserLoginDto
            {
                UserId = u.Id,
                Email = u.Email,
                Username = u.Username,
                FullName = u.FullName,
                PasswordHash = u.PasswordHash,
                UsersRoles = _context.Set<UserRole>()
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Set<Role>(), ur => ur.RoleId, r => r.Id, (ur, r) => new UserRoleDto
                    {
                        Rol = new RolDto { RolName = r.Name },
                        Permisos = _context.Set<RolePermission>()
                            .Where(rp => rp.RoleId == r.Id && rp.IsActive)
                            .Join(_context.Set<Permission>(), rp => rp.PermissionId, p => p.Id, (rp, p) => new PermissionDto
                            {
                                PermisoId = p.Id
                            })
                            .Where(p => _context.Set<Permission>().Any(perm => perm.Id == p.PermisoId && perm.IsActive))
                            .ToList()
                    }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}

public class UserLoginDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public List<UserRoleDto> UsersRoles { get; set; } = new();
}

public class UserRoleDto
{
    public RolDto Rol { get; set; } = new();
    public List<PermissionDto> Permisos { get; set; } = new();
}

public class RolDto
{
    public string RolName { get; set; } = string.Empty;
}

public class PermissionDto
{
    public Guid PermisoId { get; set; }
}
