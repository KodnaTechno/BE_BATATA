using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppGroupPermissionConfigurations: IEntityTypeConfiguration<AppGroupPermission>
{
    public void Configure(EntityTypeBuilder<AppGroupPermission> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.HasOne(a => a.AppPermission)
            .WithMany(a => a.GroupPermissions)
            .HasForeignKey(a => a.AppPermissionId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(a => a.Group)
            .WithMany()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.ToTable("appGroupPermission");
    }
}