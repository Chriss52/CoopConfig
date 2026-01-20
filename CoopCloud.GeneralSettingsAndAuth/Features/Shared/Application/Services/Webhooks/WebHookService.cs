using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.Webhooks;

/// <summary>
/// Implementation of webhook execution service
/// Handles HTTP calls to configured webhook URLs and logs execution history
/// </summary>
public class WebHookService : IWebHookService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHookConfigurationRepository _webhookRepository;
    private readonly IWebHookExecutionHistoryRepository _historyRepository;
    private readonly ILogger<WebHookService> _logger;

    public WebHookService(
        IHttpClientFactory httpClientFactory,
        IWebHookConfigurationRepository webhookRepository,
        IWebHookExecutionHistoryRepository historyRepository,
        ILogger<WebHookService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _webhookRepository = webhookRepository;
        _historyRepository = historyRepository;
        _logger = logger;
    }

    public async Task<WebHookExecutionResult> ExecuteWebhookAsync(
        Guid webhookId,
        object payload,
        CancellationToken cancellationToken = default)
    {
        var webhook = await _webhookRepository.GetByIdAsync(webhookId, cancellationToken);
        if (webhook == null)
        {
            return new WebHookExecutionResult(false, null, null, "Webhook not found");
        }

        return await ExecuteWebhookInternalAsync(webhook, payload, cancellationToken);
    }

    public async Task ExecuteWebhooksForEventAsync(
        string eventCode,
        object payload,
        CancellationToken cancellationToken = default)
    {
        var webhooks = await _webhookRepository.GetActiveByEventCodeAsync(eventCode, cancellationToken);

        var tasks = webhooks.Select(webhook => ExecuteWebhookInternalAsync(webhook, payload, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task<WebHookExecutionResult> ExecuteWebhookInternalAsync(
        WebHookConfiguration webhook,
        object payload,
        CancellationToken cancellationToken)
    {
        var history = new WebHookExecutionHistory
        {
            Id = Guid.NewGuid(),
            WebhookId = webhook.WebhookId,
            LogSent = JsonSerializer.Serialize(payload),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        try
        {
            var client = _httpClientFactory.CreateClient("WebHooks");
            var request = new HttpRequestMessage(new HttpMethod(webhook.Method), webhook.Url)
            {
                Content = new StringContent(history.LogSent, Encoding.UTF8, "application/json")
            };

            // Add custom headers if provided
            if (!string.IsNullOrWhiteSpace(webhook.Headers))
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(webhook.Headers);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            var response = await client.SendAsync(request, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            // Truncate response if too large (max 5000 chars)
            if (responseContent.Length > 5000)
            {
                responseContent = responseContent.Substring(0, 5000) + "... (truncated)";
            }

            history.HttpStatusCode = (int)response.StatusCode;
            history.LogResponse = responseContent;
            history.WasSuccessful = response.IsSuccessStatusCode;

            await _historyRepository.CreateAsync(history, cancellationToken);

            return new WebHookExecutionResult(
                response.IsSuccessStatusCode,
                (int)response.StatusCode,
                responseContent,
                null
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing webhook {WebhookId}", webhook.WebhookId);

            history.WasSuccessful = false;
            history.LogResponse = $"Error: {ex.Message}";
            await _historyRepository.CreateAsync(history, cancellationToken);

            return new WebHookExecutionResult(false, null, null, ex.Message);
        }
    }
}
