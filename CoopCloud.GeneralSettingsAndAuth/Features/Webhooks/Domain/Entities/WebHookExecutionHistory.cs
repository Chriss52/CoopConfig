namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

/// <summary>
/// WebHookExecutionHistory - Execution log for webhook calls
/// Stores the sent payload, received response, and execution result
/// NOTE: Records older than 7 days are automatically deleted
/// </summary>
public class WebHookExecutionHistory
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid WebhookId { get; set; }

    // Properties
    public string LogSent { get; set; } = string.Empty;  // Payload sent
    public string? LogResponse { get; set; }  // Response received or error message
    public int? HttpStatusCode { get; set; }  // HTTP status code from response
    public bool WasSuccessful { get; set; }  // true = success, false = failed

    // Audit Fields (different from other models - uses string for system execution)
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;  // "SYSTEM" for automatic execution
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    // Navigation Properties
    public virtual WebHookConfiguration? WebHook { get; set; }
}
