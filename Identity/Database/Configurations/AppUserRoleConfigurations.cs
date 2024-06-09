using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppUserRoleConfigurations: IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {

        builder.ToTable("AspNetUserRoles").HasKey(k => new { k.UserId, k.RoleId,k.ModuleId});
    }
}