using System.Security.Cryptography;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Security;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nubeteck.Extensions.Security;
using LoginRequest = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs.LoginRequest;
using LoginResponse = CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs.LoginResponse;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record LoginCommand(LoginRequest Request) : IRequest<LoginResponse>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly AppDbContext _context;
    private readonly JwtUtils _jwtUtils;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(AppDbContext context, JwtUtils jwtUtils, IConfiguration configuration)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _configuration = configuration;
    }

    public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var hashedPassword = _jwtUtils.EncryptToSHA256(request.Password);

        var user = await _context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.PasswordHash == hashedPassword && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new UnauthorizedAccessException("El correo y/o contraseña especificados son incorrectos");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("La cuenta de usuario está desactivada");

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate JWT token
        var userLoginDto = MapToUserLoginDto(user);
        var token = _jwtUtils.GenerateJwtToken(userLoginDto);
        var expiryMinutes = Convert.ToDouble(_configuration["JwtCredentials:ExpiryMinutes"] ?? "1440");
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // Generate and store refresh token
        var refreshToken = GenerateRefreshToken(user.Id);
        _context.Set<RefreshToken>().Add(refreshToken);

        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        return new LoginResponse(
            token,
            refreshToken.Token,
            expiresAt,
            new UserDto(
                user.Id,
                user.Email,
                user.Username,
                user.FullName,
                user.PhoneNumber,
                user.IsActive,
                user.LastLoginAt,
                user.CreatedAt,
                roles
            )
        );
    }

    private UserLoginDto MapToUserLoginDto(User user)
    {
        return new UserLoginDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            FullName = user.FullName,
            PasswordHash = user.PasswordHash,
            UsersRoles = user.UserRoles.Select(ur => new UserRoleDto
            {
                Rol = new RolDto { RolName = ur.Role.Name },
                Permisos = ur.Role.Permissions
                    .Where(rp => rp.IsActive && rp.Permission.IsActive)
                    .Select(rp => new Infrastructure.Security.PermissionDto { PermisoId = rp.Permission.Id })
                    .ToList()
            }).ToList()
        };
    }

    private RefreshToken GenerateRefreshToken(Guid userId)
    {
        var refreshTokenDays = Convert.ToInt32(_configuration["JwtCredentials:RefreshTokenExpiryDays"] ?? "7");

        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenDays),
            CreatedAt = DateTime.UtcNow
        };
    }
}
