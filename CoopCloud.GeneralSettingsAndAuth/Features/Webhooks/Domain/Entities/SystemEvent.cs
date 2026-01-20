using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

/// <summary>
/// SystemEvent - Catalog of system events that can trigger webhooks
/// Each event belongs to a category and defines the payload schema
/// </summary>
public class SystemEvent
{
    // Primary Key
    public Guid EventId { get; set; }

    // Foreign Keys
    public int CategoryId { get; set; }

    // Properties
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? PayloadSchema { get; set; }
    public bool IsActive { get; set; }

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual EventCategory? Category { get; set; }
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
    public virtual ICollection<WebHookConfiguration> WebHookConfigurations { get; set; } = new List<WebHookConfiguration>();
}
