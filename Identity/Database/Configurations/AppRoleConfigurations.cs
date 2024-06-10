using AppCommon.GlobalHelpers;
using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppRoleConfigurations: IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.Property(t => t.ExtraInfo).HasConversion(v=>v.AsText(),v=>v.FromJson<List<KeyValuePair<string, string>>>());
        builder.ComplexProperty(x => x.DisplayName);
        builder.ToTable("appRoles");
    }
}