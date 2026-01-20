using Microsoft.AspNetCore.Authorization;

namespace Nubeteck.Extensions.Security.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        var userPermissions = context.User.FindFirst("permisos")?.Value?.Split(',') ?? Array.Empty<string>();

        bool hasPermission = requirement.RequireAll
            ? requirement.Permissions.All(required => userPermissions.Contains(required))
            : requirement.Permissions.Any(required => userPermissions.Contains(required));

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
