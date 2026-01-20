using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;

public interface IWebHookExecutionHistoryRepository
{
    Task<(List<WebHookExecutionHistory> Items, int TotalCount)> GetByWebhookIdAsync(
        Guid webhookId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default);
    
    Task<WebHookExecutionHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WebHookExecutionHistory> CreateAsync(WebHookExecutionHistory history, CancellationToken cancellationToken = default);
    Task<WebHookExecutionHistory> UpdateAsync(WebHookExecutionHistory history, CancellationToken cancellationToken = default);
    Task<int> DeleteOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
    Task<(int Total, int Successful)> GetExecutionStatsAsync(Guid webhookId, DateTime fromDate, CancellationToken cancellationToken = default);
    Task<WebHookExecutionHistory?> GetLastExecutionAsync(Guid webhookId, CancellationToken cancellationToken = default);
}
