using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppAccessibilityGroupConfigurations: IEntityTypeConfiguration<AppAccessibilityGroup>
{
    public void Configure(EntityTypeBuilder<AppAccessibilityGroup> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.AppAccessibility)
            .WithMany(x => x.AccessibilityGroups)
            .HasForeignKey(x => x.AppAccessibilityId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.AppGroup)
            .WithMany()
            .HasForeignKey(x => x.AppGroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable("appAccessibilityGroup");
    }
}