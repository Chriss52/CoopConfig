using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

/// <summary>
/// GeneralSettingsCategory - Represents categories for general settings
/// </summary>
public class GeneralSettingCategory
{
    // Primary Key
    public Guid CategoryId { get; set; }

    // Properties
    public string CategoryName { get; set; } = string.Empty;

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
}
