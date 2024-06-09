using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppPermissionConfigurations: IEntityTypeConfiguration<AppPermission>
{
    public void Configure(EntityTypeBuilder<AppPermission> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.HasIndex(e => new { e.Command, e.ModuleId}).IsUnique();

        builder.ToTable("appPermissions");
    }
}