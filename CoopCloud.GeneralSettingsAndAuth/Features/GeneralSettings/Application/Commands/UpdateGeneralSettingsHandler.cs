using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using Nubeteck.Extensions.Security;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public class UpdateGeneralSettingsHandler : IRequestHandler<UpdateGeneralSettingsCommand, Unit>
{
    private readonly IGeneralSettingRepository _generalSettingsRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateGeneralSettingsHandler(
        IGeneralSettingRepository generalSettingsRepository,
        ICurrentUserService currentUserService)
    {
        _generalSettingsRepository = generalSettingsRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(UpdateGeneralSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var configurationIds = request.Settings.Select(s => s.ConfigurationId).ToList();

        var settings = await _generalSettingsRepository.GetByIdsAsync(configurationIds, cancellationToken);

        foreach (var setting in settings)
        {
            var update = request.Settings.FirstOrDefault(s => s.ConfigurationId == setting.ConfigurationId);
            if (update != null)
            {
                setting.Value = update.Value;
                setting.ModifiedBy = userId;
                setting.ModifiedDate = DateTime.UtcNow;

                await _generalSettingsRepository.UpdateAsync(setting, cancellationToken);
            }
        }

        return Unit.Value;
    }
}
