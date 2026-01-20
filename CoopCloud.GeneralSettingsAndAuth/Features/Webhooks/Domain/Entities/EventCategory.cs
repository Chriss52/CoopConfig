using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

/// <summary>
/// EventCategory - Catalog of event categories for webhooks
/// Groups related system events (e.g., Clients, Payments, Invoices)
/// </summary>
public class EventCategory
{
    // Primary Key
    public int CategoryId { get; set; }

    // Properties
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
    public virtual ICollection<SystemEvent> SystemEvents { get; set; } = new List<SystemEvent>();
}
