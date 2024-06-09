using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppUserClaimConfigurations: IEntityTypeConfiguration<AppUserClaim>
{
    public void Configure(EntityTypeBuilder<AppUserClaim> builder)
    {
        builder.ToTable("appUserClaims");
    }
}