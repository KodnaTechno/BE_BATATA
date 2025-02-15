using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;
using AppCommon.GlobalHelpers;

namespace Module.Configurations
{
    public class AppActionConfiguration : IEntityTypeConfiguration<AppAction>
    {
        public void Configure(EntityTypeBuilder<AppAction> builder)
        {
            builder.Property(e => e.Type)
                .HasConversion<string>();

            builder.Property(e => e.Name)
                .HasJsonConversion();

            builder.Property(e => e.Description)
                .HasJsonConversion();

            builder.HasOne(e => e.Module)
                .WithMany(m => m.Actions)
                .HasForeignKey(e => e.ModuleId)
                .IsRequired(false);

            builder.HasOne(e => e.Workspace)
                .WithMany(w => w.Actions)
                .HasForeignKey(e => e.WorkspaceId)
                .IsRequired(false);

            builder.HasOne(e => e.WorkspaceModule)
                .WithMany(w => w.Actions)
                .HasForeignKey(e => e.WorkspaceModuleId)
                .IsRequired(false);
        }
    }
}
