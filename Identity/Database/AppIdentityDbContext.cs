using AppIdentity.Database.Configurations;
using AppIdentity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity.Database;

public class AppIdentityDbContext : IdentityDbContext<AppUser, AppRole, Guid, AppUserClaim,
    AppUserRole, IdentityUserLogin<Guid>, AppRolePermission, IdentityUserToken<Guid>>
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppPermission> AppPermissions { get; set; }
    public DbSet<AppRolePermission> AppRolePermissions { get; set; }
    public DbSet<AppGroupPermission> AppGroupPermissions { get; set; }
    public DbSet<AppUserClaim> AppUserClaims { get; set; }
    public DbSet<AppGroup> AppGroups { get; set; }
    public DbSet<AppGroupUser> AppGroupUsers { get; set; }
    public DbSet<AppAccessibility> AppAccessibilities { get; set; }
    public DbSet<AppAccessibilityGroup> AppAccessibilityGroups { get; set; }
    public DbSet<AppUserRole> AppUserRoles { get; set; }
    public DbSet<InstanceGroupPermission> InstanceGroupPermission { get; set; }
    public DbSet<AppCredential> AppCredentials { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");
        builder.ApplyConfiguration(new AppUserConfigurations());
        builder.ApplyConfiguration(new AppRoleConfigurations());
        builder.ApplyConfiguration(new AppUserClaimConfigurations());
        builder.ApplyConfiguration(new AppPermissionConfigurations());
        builder.ApplyConfiguration(new AppRolePermissionConfigurations());
        builder.ApplyConfiguration(new AppGroupConfigurations());
        builder.ApplyConfiguration(new AppGroupUserConfigurations());
        builder.ApplyConfiguration(new AppAccessibilityConfigurations());
        builder.ApplyConfiguration(new AppAccessibilityGroupConfigurations());
        builder.ApplyConfiguration(new AppGroupPermissionConfigurations());
        builder.ApplyConfiguration(new AppUserRoleConfigurations());
        builder.ApplyConfiguration(new InstanceGroupPermissionConfigurations());
        builder.ApplyConfiguration(new AppCredentialConfigurations());

        
        var databaseProvider = Database.ProviderName;
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                // Check if the property is marked as using JSON conversion
                if (property.FindAnnotation("IsJsonConverted")?.Value as bool? == true)
                {
                    if (databaseProvider == "Pomelo.EntityFrameworkCore.MySql")
                    {
                        property.SetColumnType("LONGTEXT");
                    }
                    else if (databaseProvider == "Microsoft.EntityFrameworkCore.SqlServer")
                    {
                        property.SetColumnType("nvarchar(MAX)");
                    }
                }
            }
        }

    }


    public IEnumerable<TEntity> AddRange<TEntity>(IEnumerable<TEntity> entities, bool resumeOnException = true) where TEntity : class
    {
        if (resumeOnException)
        {
            try
            {
                base.AddRange(entities);
                base.SaveChanges();
            }
            catch (Exception exception)
            {
                SaveEntities(entities);
            }
        }
        else
        {
            base.AddRange(entities);
            base.SaveChanges();
        }

        return entities;
    }
    public IEnumerable<TEntity> SaveEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        foreach (var entity in entities)
        {
            try
            {
                base.Add(entity);
                base.SaveChanges();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"AddRangeIgnore => {exception.Message}");
                base.Entry(entity).State = EntityState.Detached;
            }
        }

        return entities;
    }

}