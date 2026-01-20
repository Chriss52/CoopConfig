using CoopCloud.GeneralSettingsAndAuth.Features.Shared;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public virtual ICollection<RolePermission> Roles { get; set; } = new List<RolePermission>();
}
