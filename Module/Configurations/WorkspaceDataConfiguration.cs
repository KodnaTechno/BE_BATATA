using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class WorkspaceDataConfiguration : IEntityTypeConfiguration<WorkspaceData>
    {
        public void Configure(EntityTypeBuilder<WorkspaceData> builder)
        {
            builder.HasOne(w => w.Workspace)
                .WithMany(w => w.WorkspaceData)
                .HasForeignKey(w => w.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(w => w.WorkspaceConnections)
                .WithOne(c => c.TargetWorkspaceData)
                .HasForeignKey(c => c.TargetWorkspaceDataId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
