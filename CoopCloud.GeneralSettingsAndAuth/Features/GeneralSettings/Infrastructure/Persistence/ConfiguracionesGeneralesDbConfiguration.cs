using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Persistence;

namespace CoopCloud.GeneralSettingsAndAuth.Features.GeneralSettings.Infrastructure.Persistence;

public class ConfiguracionesGeneralesDbConfiguration : ISubDbContext
{
    public IEnumerable<Type> GetEntities()
    {
        return new[]
        {
            typeof(GeneralSetting),
            typeof(GeneralSettingCategory),
            typeof(GeneralSettingDataType),
            typeof(GeneralSettingSource),
            typeof(GeneralSettingOption)
        };
    }

    public void ConfigureModel(ModelBuilder modelBuilder)
    {
        // GeneralSetting
        modelBuilder.Entity<GeneralSetting>(entity =>
        {
            entity.ToTable("GeneralSettings");
            entity.HasKey(e => e.ConfigurationId);

            entity.Property(e => e.Key)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Label)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Value)
                .IsRequired();

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DataType)
                .WithMany()
                .HasForeignKey(e => e.DataTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.GeneralSettingsOption)
                .WithOne(o => o.GeneralSetting)
                .HasForeignKey<GeneralSettingOption>(o => o.ConfigurationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // GeneralSettingCategory
        modelBuilder.Entity<GeneralSettingCategory>(entity =>
        {
            entity.ToTable("GeneralSettingsCategories");
            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GeneralSettingDataType
        modelBuilder.Entity<GeneralSettingDataType>(entity =>
        {
            entity.ToTable("GeneralSettingsDataTypes");
            entity.HasKey(e => e.DataTypeId);

            entity.Property(e => e.DataTypeName)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GeneralSettingSource
        modelBuilder.Entity<GeneralSettingSource>(entity =>
        {
            entity.ToTable("GeneralSettingsSources");
            entity.HasKey(e => e.SourceId);

            entity.Property(e => e.SourceName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // GeneralSettingOption
        modelBuilder.Entity<GeneralSettingOption>(entity =>
        {
            entity.ToTable("GeneralSettingsOptions");
            entity.HasKey(e => e.ConfigurationId);

            entity.HasOne(e => e.Source)
                .WithMany()
                .HasForeignKey(e => e.SourceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
