namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

public class RolePermission
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
