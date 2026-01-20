using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public record ResetAllGeneralSettingsCommand() : IRequest<Unit>;
