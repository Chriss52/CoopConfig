using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Application.Repositories;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Services.Webhooks;

/// <summary>
/// Background service that periodically deletes old webhook execution history records.
/// Records older than 7 days are automatically removed to prevent database bloat.
/// Runs every 24 hours.
/// </summary>
public class WebHookHistoryCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<WebHookHistoryCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24);
    private readonly int _retentionDays = 7;

    public WebHookHistoryCleanupService(
        IServiceProvider serviceProvider,
        ILogger<WebHookHistoryCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WebHook History Cleanup Service starting");

        // Wait 5 minutes before first cleanup
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldHistoryAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during webhook history cleanup");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupOldHistoryAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IWebHookExecutionHistoryRepository>();

        var cutoffDate = DateTime.UtcNow.AddDays(-_retentionDays);
        var deletedCount = await repository.DeleteOlderThanAsync(cutoffDate, cancellationToken);

        if (deletedCount > 0)
        {
            _logger.LogInformation("Deleted {Count} old webhook execution history records (older than {CutoffDate})", 
                deletedCount, cutoffDate);
        }
    }
}
