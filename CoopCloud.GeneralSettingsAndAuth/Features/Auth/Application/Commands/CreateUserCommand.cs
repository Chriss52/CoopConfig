using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Security;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record CreateUserCommand(CreateUserRequest Request) : IRequest<UserDto>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly AppDbContext _context;
    private readonly JwtUtils _jwtUtils;

    public CreateUserCommandHandler(AppDbContext context, JwtUtils jwtUtils)
    {
        _context = context;
        _jwtUtils = jwtUtils;
    }

    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        var existingUser = await _context.Set<User>()
            .FirstOrDefaultAsync(u => (u.Email == request.Email || u.Username == request.Username) && !u.IsDeleted, cancellationToken);

        if (existingUser != null)
        {
            if (existingUser.Email == request.Email)
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            throw new InvalidOperationException("El nombre de usuario ya está en uso");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            FullName = request.FullName,
            PasswordHash = _jwtUtils.EncryptToSHA256(request.Password),
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            StatusId = 1,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<User>().Add(user);

        // Assign roles if provided
        if (request.RoleIds != null && request.RoleIds.Any())
        {
            foreach (var roleId in request.RoleIds)
            {
                var roleExists = await _context.Set<Role>()
                    .AnyAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);

                if (roleExists)
                {
                    _context.Set<UserRole>().Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Reload with roles
        var roles = await _context.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            user.PhoneNumber,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt,
            roles
        );
    }
}
