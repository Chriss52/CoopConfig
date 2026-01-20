using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Repositories;
using MediatR;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.Queries;

public class GetAllGeneralSettingsHandler : IRequestHandler<GetAllGeneralSettingsQuery, List<GeneralSettingDto>>
{
    private readonly IGeneralSettingRepository _generalSettingsRepository;

    public GetAllGeneralSettingsHandler(IGeneralSettingRepository generalSettingsRepository)
    {
        _generalSettingsRepository = generalSettingsRepository;
    }

    public async Task<List<GeneralSettingDto>> Handle(GetAllGeneralSettingsQuery request, CancellationToken cancellationToken)
    {
        var generalSettings = await _generalSettingsRepository.GetAllAsync(cancellationToken);

        return generalSettings.Select(s => new GeneralSettingDto(
            s.ConfigurationId,
            s.Key,
            s.Label,
            s.Value,
            s.DefaultValue,
            s.Description,
            s.DataTypeId,
            s.DataType?.DataTypeName ?? string.Empty,
            s.CategoryId,
            s.Category?.CategoryName ?? string.Empty,
            s.Required,
            s.Rules
        )).ToList();
    }
}
