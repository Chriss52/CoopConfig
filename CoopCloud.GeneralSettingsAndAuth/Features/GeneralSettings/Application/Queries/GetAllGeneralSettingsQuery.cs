using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Queries;

public record GetAllGeneralSettingsQuery : IRequest<List<GeneralSettingDto>>;
