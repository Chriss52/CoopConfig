using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Nubeteck.Extensions.Security;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _hostEnvironment;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IHostEnvironment hostEnvironment)
    {
        _httpContextAccessor = httpContextAccessor;
        _hostEnvironment = hostEnvironment;
    }

    public Guid? UserId
    {
        get
        {
            var userIdValue = _httpContextAccessor?.HttpContext?.User.FindFirst("userId")?.Value;

            if (Guid.TryParse(userIdValue, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}
