using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using Nubeteck.Extensions.Security;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public class ResetGeneralSettingHandler : IRequestHandler<ResetGeneralSettingCommand, Unit>
{
    private readonly IGeneralSettingRepository _generalSettingsRepository;
    private readonly ICurrentUserService _currentUserService;

    public ResetGeneralSettingHandler(
        IGeneralSettingRepository generalSettingsRepository,
        ICurrentUserService currentUserService)
    {
        _generalSettingsRepository = generalSettingsRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Unit> Handle(ResetGeneralSettingCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var setting = await _generalSettingsRepository.GetByIdAsync(request.ConfigurationId, cancellationToken);

        if (setting == null)
        {
            throw new KeyNotFoundException($"Setting with ID {request.ConfigurationId} not found");
        }

        if (!string.IsNullOrEmpty(setting.DefaultValue))
        {
            setting.Value = setting.DefaultValue;
            setting.ModifiedBy = userId;
            setting.ModifiedDate = DateTime.UtcNow;

            await _generalSettingsRepository.UpdateAsync(setting, cancellationToken);
        }

        return Unit.Value;
    }
}
