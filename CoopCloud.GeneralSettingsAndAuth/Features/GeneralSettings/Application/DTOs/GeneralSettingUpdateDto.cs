namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;

public record GeneralSettingUpdateDto(
    Guid ConfigurationId,
    string Value
);
