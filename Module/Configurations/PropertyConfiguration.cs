using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;

namespace Module.Configurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.Property(e => e.ViewType).HasConversion<string>();
            builder.Property(e => e.DataType).HasConversion<string>();

            builder.HasOne(e => e.Module)
                .WithMany(m => m.Properties)  
                .HasForeignKey(e => e.ModuleId);

            builder.HasOne(e => e.Workspace)
                .WithMany(w => w.Properties)  
                .HasForeignKey(e => e.WorkspaceId);

            builder.HasOne(e => e.WorkspaceModule)
                .WithMany(wm => wm.Properties)  
                .HasForeignKey(e => e.WorkspaceModuleId);
        }
    }

}
