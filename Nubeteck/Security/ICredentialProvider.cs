namespace Nubeteck.Extensions.Security;

public interface ICredentialProvider
{
    Task<bool> IsValidAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<UserInfo> GetUserInfoAsync(string email, CancellationToken cancellationToken = default);
    Task<string> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<string> LoginActiveDirectoryAsync(string email, string password, CancellationToken cancellationToken = default);

    // Backward compatibility
    bool IsValid(string email, string password);
    UserInfo GetUserInfo(string email);
}
