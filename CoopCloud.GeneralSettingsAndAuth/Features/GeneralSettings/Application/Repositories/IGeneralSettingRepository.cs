using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;

public interface IGeneralSettingRepository
{
    Task<List<GeneralSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GeneralSetting?> GetByIdAsync(Guid configurationId, CancellationToken cancellationToken = default);
    Task<GeneralSettingOption?> GetOptionByIdAsync(Guid configurationId, CancellationToken cancellationToken = default);
    Task UpdateAsync(GeneralSetting generalSetting, CancellationToken cancellationToken = default);
    Task<List<GeneralSetting>> GetByIdsAsync(List<Guid> configurationIds, CancellationToken cancellationToken = default);
}
