using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public record ResetGeneralSettingCommand(Guid ConfigurationId) : IRequest<Unit>;
