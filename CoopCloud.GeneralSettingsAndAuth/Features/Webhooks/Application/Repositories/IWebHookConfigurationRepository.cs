using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;

public interface IWebHookConfigurationRepository
{
    Task<List<WebHookConfiguration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WebHookConfiguration?> GetByIdAsync(Guid webhookId, CancellationToken cancellationToken = default);
    Task<List<WebHookConfiguration>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<List<WebHookConfiguration>> GetActiveByEventCodeAsync(string eventCode, CancellationToken cancellationToken = default);
    Task<WebHookConfiguration> CreateAsync(WebHookConfiguration webhook, CancellationToken cancellationToken = default);
    Task<WebHookConfiguration> UpdateAsync(WebHookConfiguration webhook, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid webhookId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid webhookId, CancellationToken cancellationToken = default);
    Task<bool> ExistsDuplicateAsync(Guid eventId, string url, Guid? excludeWebhookId = null, CancellationToken cancellationToken = default);
}
