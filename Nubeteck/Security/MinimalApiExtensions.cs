using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nubeteck.Extensions.Security.Authorization;

namespace Nubeteck.Extensions.Security;

public static class MinimalApiExtensions
{
    public static RouteHandlerBuilder RequirePermission(this RouteHandlerBuilder builder, params string[] permissions)
    {
        var policyName = $"RequirePermission_{string.Join("_", permissions)}";
        return builder.RequireAuthorization(policyName);
    }

    public static RouteHandlerBuilder RequireAllPermissions(this RouteHandlerBuilder builder, params string[] permissions)
    {
        var policyName = $"RequireAllPermissions_{string.Join("_", permissions)}";
        return builder.RequireAuthorization(policyName);
    }

    public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddAuthorizationBuilder()
            .AddPolicy("RequireAnyPermission", policy =>
                policy.Requirements.Add(new PermissionRequirement(Array.Empty<string>(), false)))
            .AddPolicy("RequireAllPermissions", policy =>
                policy.Requirements.Add(new PermissionRequirement(Array.Empty<string>(), true)));

        return services;
    }

    public static void ConfigurePermissionPolicies(this IServiceCollection services)
    {
        services.Configure<AuthorizationOptions>(options =>
        {
            options.FallbackPolicy = null;
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    }
}

public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null)
        {
            return policy;
        }

        if (policyName.StartsWith("RequirePermission_"))
        {
            var permissionsStr = policyName.Substring("RequirePermission_".Length);
            var permissions = permissionsStr.Split('_');
            var requireAll = policyName.StartsWith("RequireAllPermissions_");

            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permissions, requireAll))
                .Build();
        }

        if (policyName.StartsWith("RequireAllPermissions_"))
        {
            var permissionsStr = policyName.Substring("RequireAllPermissions_".Length);
            var permissions = permissionsStr.Split('_');

            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(permissions, true))
                .Build();
        }

        return null;
    }
}
