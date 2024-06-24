using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class WorkspaceModuleConfiguration : IEntityTypeConfiguration<WorkspaceModule>
    {
        public void Configure(EntityTypeBuilder<WorkspaceModule> builder)
        {
            builder.HasOne(e => e.Workspace)
                .WithMany(e => e.WorkspaceModules)
                .HasForeignKey(e => e.WorkspaceId)
                .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(e => e.Module)
                   .WithMany(e => e.WorkspaceModules)
                   .HasForeignKey(e => e.ModuleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
