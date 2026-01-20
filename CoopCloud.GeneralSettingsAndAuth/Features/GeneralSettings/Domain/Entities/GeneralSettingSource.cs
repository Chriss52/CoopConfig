using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

/// <summary>
/// GeneralSettingsSource - Represents sources for general settings options
/// </summary>
public class GeneralSettingSource
{
    // Primary Key
    public Guid SourceId { get; set; }

    // Properties
    public string SourceName { get; set; } = string.Empty;

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
}
