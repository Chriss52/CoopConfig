using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Infrastructure.Repositories;

public class WebHookExecutionHistoryRepository : IWebHookExecutionHistoryRepository
{
    private readonly AppDbContext _context;

    public WebHookExecutionHistoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<WebHookExecutionHistory> Items, int TotalCount)> GetByWebhookIdAsync(
        Guid webhookId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<WebHookExecutionHistory>()
            .Where(h => h.WebhookId == webhookId)
            .OrderByDescending(h => h.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<WebHookExecutionHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookExecutionHistory>()
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public async Task<WebHookExecutionHistory> CreateAsync(WebHookExecutionHistory history, CancellationToken cancellationToken = default)
    {
        _context.Set<WebHookExecutionHistory>().Add(history);
        await _context.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<WebHookExecutionHistory> UpdateAsync(WebHookExecutionHistory history, CancellationToken cancellationToken = default)
    {
        _context.Set<WebHookExecutionHistory>().Update(history);
        await _context.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<int> DeleteOlderThanAsync(DateTime cutoffDate, CancellationToken cancellationToken = default)
    {
        var oldRecords = await _context.Set<WebHookExecutionHistory>()
            .Where(h => h.CreatedAt < cutoffDate)
            .ToListAsync(cancellationToken);

        _context.Set<WebHookExecutionHistory>().RemoveRange(oldRecords);
        await _context.SaveChangesAsync(cancellationToken);
        
        return oldRecords.Count;
    }

    public async Task<(int Total, int Successful)> GetExecutionStatsAsync(
        Guid webhookId,
        DateTime fromDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<WebHookExecutionHistory>()
            .Where(h => h.WebhookId == webhookId && h.CreatedAt >= fromDate);

        var total = await query.CountAsync(cancellationToken);
        var successful = await query.CountAsync(h => h.WasSuccessful, cancellationToken);

        return (total, successful);
    }

    public async Task<WebHookExecutionHistory?> GetLastExecutionAsync(Guid webhookId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookExecutionHistory>()
            .Where(h => h.WebhookId == webhookId)
            .OrderByDescending(h => h.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
