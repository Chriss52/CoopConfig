using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using Nubeteck.Extensions.Security;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public class ResetAllGeneralSettingsHandler : IRequestHandler<ResetAllGeneralSettingsCommand, Unit>
{
    private readonly IGeneralSettingRepository _generalSettingsRepository;
    private readonly ICurrentUserService _currentUserService;

    public ResetAllGeneralSettingsHandler(
        IGeneralSettingRepository generalSettingsRepository,
        ICurrentUserService currentUserService)
    {
        _generalSettingsRepository = generalSettingsRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ResetAllGeneralSettingsCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var settings = await _generalSettingsRepository.GetAllAsync(cancellationToken);

        foreach (var setting in settings.Where(s => !string.IsNullOrEmpty(s.DefaultValue)))
        {
            setting.Value = setting.DefaultValue!;
            setting.ModifiedBy = userId;
            setting.ModifiedDate = DateTime.UtcNow;

            await _generalSettingsRepository.UpdateAsync(setting, cancellationToken);
        }

        return Unit.Value;
    }
}
