using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Commands;

public record UpdateGeneralSettingsCommand(
    List<GeneralSettingUpdateDto> Settings
) : IRequest<Unit>;
