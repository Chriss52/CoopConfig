using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.Queries;

public record GetUserInfoQuery(Guid UserId, string? Scope) : IRequest<UserInfoResponse?>;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfoResponse?>
{
    private readonly AppDbContext _context;

    public GetUserInfoQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserInfoResponse?> Handle(GetUserInfoQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.Permissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == query.UserId && !u.IsDeleted, cancellationToken);

        if (user == null)
            return null;

        var scopes = (query.Scope ?? "openid").Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string? email = null;
        bool? emailVerified = null;
        string? name = null;
        string? preferredUsername = null;
        string? phoneNumber = null;
        IEnumerable<string>? roles = null;
        IEnumerable<string>? permissions = null;

        if (scopes.Contains("email"))
        {
            email = user.Email;
            emailVerified = true;
        }

        if (scopes.Contains("profile"))
        {
            name = user.FullName;
            preferredUsername = user.Username;
            phoneNumber = user.PhoneNumber;
        }

        if (scopes.Contains("roles") || scopes.Contains("openid"))
        {
            roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        }

        if (scopes.Contains("permissions") || scopes.Contains("openid"))
        {
            permissions = user.UserRoles
                .SelectMany(ur => ur.Role.Permissions)
                .Where(rp => rp.IsActive && rp.Permission.IsActive)
                .Select(rp => rp.Permission.Key)
                .Distinct()
                .ToList();
        }

        return new UserInfoResponse(
            Sub: user.Id.ToString(),
            Email: email,
            EmailVerified: emailVerified,
            Name: name,
            PreferredUsername: preferredUsername,
            Picture: null,
            PhoneNumber: phoneNumber,
            Roles: roles,
            Permissions: permissions
        );
    }
}
