using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

/// <summary>
/// GeneralSettingsOption - Represents options for general settings (one-to-one with GeneralSetting)
/// </summary>
public class GeneralSettingOption
{
    // Primary Key and Foreign Key (One-to-One)
    public Guid ConfigurationId { get; set; }

    // Properties
    public Guid SourceId { get; set; }
    public string? Configuration { get; set; }
    public bool? AllowMultiple { get; set; }

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual GeneralSetting? GeneralSetting { get; set; }
    public virtual GeneralSettingSource? Source { get; set; }
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
}
