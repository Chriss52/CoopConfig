namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Application.DTOs;

public class UpdateSettingRangeDto
{
    public int SettingId { get; set; }
    public string Value { get; set; } = string.Empty;
}
