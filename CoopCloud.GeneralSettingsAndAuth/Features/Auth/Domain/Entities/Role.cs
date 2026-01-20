using CoopCloud.GeneralSettingsAndAuth.Features.Shared;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}
