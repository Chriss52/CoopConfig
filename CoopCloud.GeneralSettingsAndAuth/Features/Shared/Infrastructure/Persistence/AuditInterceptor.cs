using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Common;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Services;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IAuditUserService _auditUserService;

    public AuditInterceptor(IAuditUserService auditUserService)
    {
        _auditUserService = auditUserService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditProperties(eventData.Context!);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetAuditProperties(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditProperties(DbContext context)
    {
        var currentUserId = _auditUserService.GetCurrentUserId();
        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Entity.UpdatedBy = currentUserId;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = now;
                    entry.Entity.DeletedBy = currentUserId;
                    break;
            }
        }
    }
}
