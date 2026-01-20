using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Services;

public interface IAuditUserService
{
    Guid GetCurrentUserId();
}

public class AuditUserService : IAuditUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static readonly Guid SystemUserId = Guid.Parse("d7dc0f9a-dddc-4e0b-b483-4380b1c4a6ae");

    public Guid GetCurrentUserId()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)
                           ?? user.FindFirst("sub")
                           ?? user.FindFirst("userId");

            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }

        return SystemUserId;
    }
}
