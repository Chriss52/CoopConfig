using System.Text.Json.Serialization;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
