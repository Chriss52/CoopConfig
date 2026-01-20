using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

/// <summary>
/// WebHookConfiguration - Configuration for a webhook associated with a system event
/// Defines the URL, headers, and HTTP method to notify when an event occurs
/// </summary>
public class WebHookConfiguration
{
    // Primary Key
    public Guid WebhookId { get; set; }

    // Foreign Keys
    public Guid EventId { get; set; }

    // Properties
    public string Url { get; set; } = string.Empty;
    public string? Headers { get; set; }  // JSON format: {"Authorization": "Bearer xxx"}
    public string Method { get; set; } = string.Empty;  // POST, PUT, PATCH
    public bool IsActive { get; set; }

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual SystemEvent? Event { get; set; }
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
    public virtual ICollection<WebHookExecutionHistory> ExecutionHistory { get; set; } = new List<WebHookExecutionHistory>();
}
