using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppAccessibilityConfigurations: IEntityTypeConfiguration<AppAccessibility>
{
    public void Configure(EntityTypeBuilder<AppAccessibility> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();

        builder.HasMany(x => x.AccessibilityGroups)
            .WithOne(x => x.AppAccessibility);

        builder.HasIndex(a => a.ModuleKey).IsUnique();
        
        builder.ToTable("appAccessibility");
    }
}