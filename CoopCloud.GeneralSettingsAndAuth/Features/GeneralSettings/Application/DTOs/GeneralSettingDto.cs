namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;

public record GeneralSettingDto(
    Guid ConfigurationId,
    string Key,
    string Label,
    string Value,
    string? DefaultValue,
    string Description,
    Guid DataTypeId,
    string DataTypeName,
    Guid CategoryId,
    string CategoryName,
    bool Required,
    string? Rules
);
