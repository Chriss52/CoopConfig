using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Common;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared;

public class BaseEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public int StatusId { get; set; }
    public virtual Status Status { get; set; } = null!;
}
