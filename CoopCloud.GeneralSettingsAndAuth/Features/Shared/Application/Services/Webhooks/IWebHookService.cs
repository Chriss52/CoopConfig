namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.Webhooks;

/// <summary>
/// Service interface for executing webhooks
/// Handles HTTP calls to configured webhook URLs when events occur
/// </summary>
public interface IWebHookService
{
    /// <summary>
    /// Executes a specific webhook with the provided payload
    /// </summary>
    Task<WebHookExecutionResult> ExecuteWebhookAsync(Guid webhookId, object payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes all active webhooks configured for a specific event code
    /// </summary>
    Task ExecuteWebhooksForEventAsync(string eventCode, object payload, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a webhook execution
/// </summary>
public record WebHookExecutionResult(
    bool Success,
    int? HttpStatusCode,
    string? Response,
    string? ErrorMessage
);
