using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppCredentialConfigurations: IEntityTypeConfiguration<AppCredential>
{
    public void Configure(EntityTypeBuilder<AppCredential> builder)
    {
        builder.HasKey(t => t.AppId);
        builder.ToTable("appCredentials");
    }
}