using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

/// <summary>
/// GeneralSetting - Represents general system configuration settings
/// </summary>
public class GeneralSetting
{
    // Primary Key
    public Guid ConfigurationId { get; set; }

    // Properties
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? DefaultValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid DataTypeId { get; set; }
    public Guid CategoryId { get; set; }
    public bool Required { get; set; }
    public string? Rules { get; set; }

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual GeneralSettingDataType? DataType { get; set; }
    public virtual GeneralSettingCategory? Category { get; set; }
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
    public virtual GeneralSettingOption? GeneralSettingsOption { get; set; }
}
