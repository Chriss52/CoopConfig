using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Persistence;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Webhooks.Infrastructure.Persistence;

public class WebhooksDbConfiguration : ISubDbContext
{
    public IEnumerable<Type> GetEntities()
    {
        return new[]
        {
            typeof(EventCategory),
            typeof(SystemEvent),
            typeof(WebHookConfiguration),
            typeof(WebHookExecutionHistory)
        };
    }

    public void ConfigureModel(ModelBuilder modelBuilder)
    {
        // EventCategory
        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.ToTable("EventCategories");
            entity.HasKey(e => e.CategoryId);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Icon).HasMaxLength(50);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // SystemEvent
        modelBuilder.Entity<SystemEvent>(entity =>
        {
            entity.ToTable("SystemEvents");
            entity.HasKey(e => e.EventId);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.SystemEvents)
                .HasForeignKey(e => e.CategoryId)
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

        // WebHookConfiguration
        modelBuilder.Entity<WebHookConfiguration>(entity =>
        {
            entity.ToTable("WebHookConfigurations");
            entity.HasKey(e => e.WebhookId);

            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Method).IsRequired().HasMaxLength(10);

            entity.HasOne(e => e.Event)
                .WithMany(e => e.WebHookConfigurations)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Modifier)
                .WithMany()
                .HasForeignKey(e => e.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // WebHookExecutionHistory
        modelBuilder.Entity<WebHookExecutionHistory>(entity =>
        {
            entity.ToTable("WebHookExecutionHistories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.LogSent).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.WebHook)
                .WithMany(w => w.ExecutionHistory)
                .HasForeignKey(e => e.WebhookId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
