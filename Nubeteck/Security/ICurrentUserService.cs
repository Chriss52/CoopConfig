namespace Nubeteck.Extensions.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}
