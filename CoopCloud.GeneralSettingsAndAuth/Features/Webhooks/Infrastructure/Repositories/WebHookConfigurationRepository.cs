using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Infrastructure.Repositories;

public class WebHookConfigurationRepository : IWebHookConfigurationRepository
{
    private readonly AppDbContext _context;

    public WebHookConfigurationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WebHookConfiguration>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookConfiguration>()
            .Include(w => w.Event)
            .ToListAsync(cancellationToken);
    }

    public async Task<WebHookConfiguration?> GetByIdAsync(Guid webhookId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookConfiguration>()
            .Include(w => w.Event)
            .FirstOrDefaultAsync(w => w.WebhookId == webhookId, cancellationToken);
    }

    public async Task<List<WebHookConfiguration>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookConfiguration>()
            .Include(w => w.Event)
            .Where(w => w.EventId == eventId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WebHookConfiguration>> GetActiveByEventCodeAsync(string eventCode, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookConfiguration>()
            .Include(w => w.Event)
            .Where(w => w.IsActive && w.Event != null && w.Event.Code == eventCode && w.Event.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<WebHookConfiguration> CreateAsync(WebHookConfiguration webhook, CancellationToken cancellationToken = default)
    {
        _context.Set<WebHookConfiguration>().Add(webhook);
        await _context.SaveChangesAsync(cancellationToken);
        return webhook;
    }

    public async Task<WebHookConfiguration> UpdateAsync(WebHookConfiguration webhook, CancellationToken cancellationToken = default)
    {
        _context.Set<WebHookConfiguration>().Update(webhook);
        await _context.SaveChangesAsync(cancellationToken);
        return webhook;
    }

    public async Task<bool> DeleteAsync(Guid webhookId, CancellationToken cancellationToken = default)
    {
        var webhook = await _context.Set<WebHookConfiguration>()
            .FirstOrDefaultAsync(w => w.WebhookId == webhookId, cancellationToken);

        if (webhook == null) return false;

        _context.Set<WebHookConfiguration>().Remove(webhook);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(Guid webhookId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<WebHookConfiguration>()
            .AnyAsync(w => w.WebhookId == webhookId, cancellationToken);
    }

    public async Task<bool> ExistsDuplicateAsync(
        Guid eventId,
        string url,
        Guid? excludeWebhookId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Set<WebHookConfiguration>()
            .Where(w => w.EventId == eventId && w.Url == url);

        if (excludeWebhookId.HasValue)
        {
            query = query.Where(w => w.WebhookId != excludeWebhookId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
