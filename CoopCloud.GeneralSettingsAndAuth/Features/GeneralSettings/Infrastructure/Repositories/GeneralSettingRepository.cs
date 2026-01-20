using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Infrastructure.Repositories;

public class GeneralSettingRepository : IGeneralSettingRepository
{
    private readonly AppDbContext _context;

    public GeneralSettingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GeneralSetting>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<GeneralSetting>()
            .Include(s => s.Category)
            .Include(s => s.DataType)
            .Include(s => s.GeneralSettingsOption)
                .ThenInclude(o => o!.Source)
            .ToListAsync(cancellationToken);
    }

    public async Task<GeneralSetting?> GetByIdAsync(Guid configurationId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<GeneralSetting>()
            .Include(s => s.Category)
            .Include(s => s.DataType)
            .Include(s => s.GeneralSettingsOption)
                .ThenInclude(o => o!.Source)
            .FirstOrDefaultAsync(s => s.ConfigurationId == configurationId, cancellationToken);
    }

    public async Task<GeneralSettingOption?> GetOptionByIdAsync(
        Guid configurationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<GeneralSettingOption>()
            .Include(o => o.Source)
            .Include(o => o.GeneralSetting)
            .FirstOrDefaultAsync(o => o.ConfigurationId == configurationId, cancellationToken);
    }

    public async Task UpdateAsync(GeneralSetting generalSetting, CancellationToken cancellationToken = default)
    {
        _context.Set<GeneralSetting>().Update(generalSetting);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<GeneralSetting>> GetByIdsAsync(
        List<Guid> configurationIds,
        CancellationToken cancellationToken = default)
    {
        return await _context.Set<GeneralSetting>()
            .Include(s => s.Category)
            .Include(s => s.DataType)
            .Include(s => s.GeneralSettingsOption)
                .ThenInclude(o => o!.Source)
            .Where(s => configurationIds.Contains(s.ConfigurationId))
            .ToListAsync(cancellationToken);
    }
}
