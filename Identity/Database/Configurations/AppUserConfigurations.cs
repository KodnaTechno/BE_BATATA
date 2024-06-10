using AppCommon.GlobalHelpers;
using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppUserConfigurations: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(t => t.ExtraInfo).HasConversion(v => v.AsText(), v => v.FromJson<List<KeyValuePair<string, string>>>()); ;
        builder.Property(t => t.ProviderExtraInfo).HasConversion(v => v.AsText(), v => v.FromJson<List<KeyValuePair<string, string>>>()); ;
        builder.Property(t => t.Image).IsRequired(false);
        builder
            .ToTable("appUsers");
    }
}