using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    private readonly AppDbContext _context;

    public GetAllUsersQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync(cancellationToken);

        return users.Select(u => new UserDto(
            u.Id,
            u.Email,
            u.Username,
            u.FullName,
            u.PhoneNumber,
            u.IsActive,
            u.LastLoginAt,
            u.CreatedAt,
            u.UserRoles.Select(ur => ur.Role.Name)
        ));
    }
}
