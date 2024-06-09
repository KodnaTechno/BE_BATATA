using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class InstanceGroupPermissionConfigurations : IEntityTypeConfiguration<InstanceGroupPermission>
{
    public void Configure(EntityTypeBuilder<InstanceGroupPermission> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(a => a.Group)
            .WithMany()
            .HasForeignKey(a => a.GroupId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.ToTable("instanceGroupPermission");
    }
}