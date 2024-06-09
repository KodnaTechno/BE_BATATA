using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppRoleConfigurations: IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.Property(t => t.ExtraInfo);
        builder.Property(t => t.DisplayName);
        builder.ToTable("appRoles");
    }
}