using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;

/// <summary>
/// GeneralSettingsDataType - Represents data types for general settings
/// </summary>
public class GeneralSettingDataType
{
    // Primary Key
    public Guid DataTypeId { get; set; }

    // Properties
    public string DataTypeName { get; set; } = string.Empty;

    // Audit Fields
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation Properties
    public virtual User? Creator { get; set; }
    public virtual User? Modifier { get; set; }
}
