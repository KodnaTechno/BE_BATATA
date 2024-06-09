using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppUserConfigurations: IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(t => t.ExtraInfo);
        builder.Property(t => t.ProviderExtraInfo);
        builder.Property(t => t.Image).IsRequired(false);
        builder
            .ToTable("appUsers");
    }
}