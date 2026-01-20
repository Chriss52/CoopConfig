using Nubeteck.Extensions.Persistence;
using Microsoft.EntityFrameworkCore;
using MediatR;
using CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Common;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator, AuditInterceptor auditInterceptor) : DbContext(options)
{
    private readonly IMediator _mediator = mediator;
    private readonly AuditInterceptor _auditInterceptor = auditInterceptor;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.RegisterSubModels();
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("D7DC0F9A-DDDC-4E0B-B483-4380B1C4A6AE");
        var seedDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        var statusId = 1;

        // Statuses
        modelBuilder.Entity<Domain.Entities.Status>().HasData(
            new { Id = 1, Name = "Activo", Code = "ACT", Description = "Registro activo", IsActive = true },
            new { Id = 2, Name = "Inactivo", Code = "INA", Description = "Registro inactivo", IsActive = true },
            new { Id = 3, Name = "Bloqueado", Code = "BLQ", Description = "Registro bloqueado", IsActive = true }
        );

        // Admin User
        modelBuilder.Entity<Auth.Domain.Entities.User>().HasData(
            new
            {
                Id = adminId,
                Email = "admin@example.com",
                Username = "admin",
                FullName = "Administrador",
                PasswordHash = "c775e7b757ede630cd0aa1113bd102661ab38829ca52a6422ab782862f268646", // SHA256("1234567890")
                IsActive = true,
                PhoneNumber = (string?)null,
                LastLoginAt = (DateTime?)null,
                CreatedAt = seedDate,
                UpdatedAt = (DateTime?)null,
                CreatedBy = adminId,
                UpdatedBy = (Guid?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null,
                DeletedBy = (Guid?)null,
                StatusId = statusId
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEntities = ChangeTracker
            .Entries<AggregateRoot<object>>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        var result = await base.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditInterceptor);
    }
}
