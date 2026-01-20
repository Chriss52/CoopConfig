using Microsoft.AspNetCore.Authorization;

namespace Nubeteck.Extensions.Security.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }
    public bool RequireAll { get; }

    public PermissionRequirement(string[] permissions, bool requireAll = false)
    {
        Permissions = permissions;
        RequireAll = requireAll;
    }
}
