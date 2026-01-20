namespace Nubeteck.Extensions.Security;

public sealed class UserInfo
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public List<string> Roles { get; set; } = [];
    public List<string> Permissions { get; set; } = [];
    public dynamic? UsersRoles { get; set; }
}
