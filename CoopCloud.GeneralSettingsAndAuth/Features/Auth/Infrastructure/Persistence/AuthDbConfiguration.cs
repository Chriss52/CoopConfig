using CoopCloud.GeneralSettingsAndAuth.Features.Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Nubeteck.Extensions.Persistence;

namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Infrastructure.Persistence;

public class AuthDbConfiguration : ISubDbContext
{
    public IEnumerable<Type> GetEntities()
    {
        return new[]
        {
            typeof(Role),
            typeof(Permission),
            typeof(UserRole),
            typeof(RolePermission),
            typeof(RefreshToken),
            typeof(Client),
            typeof(ClientRedirectUri),
            typeof(AuthorizationCode)
        };
    }

    public void ConfigureModel(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Name).IsUnique();

            entity.HasOne(e => e.Status)
                  .WithMany()
                  .HasForeignKey(e => e.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.DeletedAt);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Key).IsRequired().HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.HasIndex(e => e.Key).IsUnique();

            entity.HasOne(e => e.Status)
                  .WithMany()
                  .HasForeignKey(e => e.StatusId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserRoles)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("RolePermissions");
            entity.HasKey(e => new { e.RoleId, e.PermissionId });

            entity.HasOne(e => e.Role)
                  .WithMany(r => r.Permissions)
                  .HasForeignKey(e => e.RoleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                  .WithMany(p => p.Roles)
                  .HasForeignKey(e => e.PermissionId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsRevoked).HasDefaultValue(false);
            entity.Property(e => e.ReplacedByToken).HasMaxLength(500);
            entity.Property(e => e.RevokedReason).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Token).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // OAuth2 Client configuration
        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ClientSecret).HasMaxLength(500);
            entity.Property(e => e.ClientType).IsRequired().HasMaxLength(50).HasDefaultValue("confidential");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
            entity.Property(e => e.HomepageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.AccessTokenLifetimeMinutes).HasDefaultValue(60);
            entity.Property(e => e.RefreshTokenLifetimeDays).HasDefaultValue(7);
            entity.Property(e => e.RequirePkce).HasDefaultValue(false);
            entity.Property(e => e.AllowedGrantTypes).IsRequired().HasMaxLength(500).HasDefaultValue("authorization_code,refresh_token");
            entity.Property(e => e.AllowedScopes).IsRequired().HasMaxLength(500).HasDefaultValue("openid,profile,email");
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.ClientId).IsUnique();

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Client Redirect URIs configuration
        modelBuilder.Entity<ClientRedirectUri>(entity =>
        {
            entity.ToTable("ClientRedirectUris");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Uri).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => new { e.ClientId, e.Uri }).IsUnique();

            entity.HasOne(e => e.Client)
                  .WithMany(c => c.RedirectUris)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Authorization Code configuration
        modelBuilder.Entity<AuthorizationCode>(entity =>
        {
            entity.ToTable("AuthorizationCodes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code).IsRequired().HasMaxLength(500);
            entity.Property(e => e.RedirectUri).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Scopes).IsRequired().HasMaxLength(500);
            entity.Property(e => e.CodeChallenge).HasMaxLength(500);
            entity.Property(e => e.CodeChallengeMethod).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(500);
            entity.Property(e => e.Nonce).HasMaxLength(500);
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.IsUsed).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Code).IsUnique();

            entity.HasOne(e => e.Client)
                  .WithMany(c => c.AuthorizationCodes)
                  .HasForeignKey(e => e.ClientId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CreatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.UpdatedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.UpdatedBy)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DeletedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.DeletedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
