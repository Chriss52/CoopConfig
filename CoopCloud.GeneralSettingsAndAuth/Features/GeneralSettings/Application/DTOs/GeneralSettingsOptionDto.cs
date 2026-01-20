using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.DTOs;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;

public record GeneralSettingsOptionDto(
    Guid ConfigurationId,
    Guid SourceId,
    string SourceName,
    List<OptionValueDto> Options,
    bool AllowMultiple
);
