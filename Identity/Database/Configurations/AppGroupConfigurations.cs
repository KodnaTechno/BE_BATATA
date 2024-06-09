using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppGroupConfigurations: IEntityTypeConfiguration<AppGroup>
{
    public void Configure(EntityTypeBuilder<AppGroup> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.HasMany(x => x.GroupUsers)
            .WithOne(x => x.Group);

        builder.ToTable("appGroups");
    }
}