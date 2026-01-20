using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Commands;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Request) : IRequest<UserDto>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    private readonly AppDbContext _context;

    public UpdateUserCommandHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == command.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            throw new InvalidOperationException("Usuario no encontrado");

        var request = command.Request;

        // Check for duplicates if email or username is being updated
        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            var emailExists = await _context.Set<User>()
                .AnyAsync(u => u.Email == request.Email && u.Id != user.Id && !u.IsDeleted, cancellationToken);
            if (emailExists)
                throw new InvalidOperationException("El correo electrónico ya está registrado");
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
        {
            var usernameExists = await _context.Set<User>()
                .AnyAsync(u => u.Username == request.Username && u.Id != user.Id && !u.IsDeleted, cancellationToken);
            if (usernameExists)
                throw new InvalidOperationException("El nombre de usuario ya está en uso");
            user.Username = request.Username;
        }

        if (!string.IsNullOrEmpty(request.FullName))
            user.FullName = request.FullName;

        if (request.PhoneNumber != null)
            user.PhoneNumber = request.PhoneNumber;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UserDto(
            user.Id,
            user.Email,
            user.Username,
            user.FullName,
            user.PhoneNumber,
            user.IsActive,
            user.LastLoginAt,
            user.CreatedAt,
            user.UserRoles.Select(ur => ur.Role.Name)
        );
    }
}
