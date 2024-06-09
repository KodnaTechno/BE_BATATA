using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppRolePermissionConfigurations: IEntityTypeConfiguration<AppRolePermission>
{
    public void Configure(EntityTypeBuilder<AppRolePermission> builder)
    {
        builder.HasOne(a => a.AppPermission)
            .WithMany(a => a.RolePermissions)
            .HasForeignKey(a => a.AppPermissionId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.ToTable("appRolePermission");
    }
}