using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class WorkspaceConnectionDataConfiguration : IEntityTypeConfiguration<WorkspaceConnectionData>
    {
        public void Configure(EntityTypeBuilder<WorkspaceConnectionData> builder)
        {
            builder.HasOne(w => w.WorkspaceConnection)
                .WithMany(w => w.WorkspaceConnectionData)
                .HasForeignKey(w => w.WorkspaceConnectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(w => w.SourceWorkspaceData)
                .WithMany()
                .HasForeignKey(w => w.SourceWorkspaceDataId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.TargetWorkspaceData)
                .WithMany(w => w.WorkspaceConnections)
                .HasForeignKey(w => w.TargetWorkspaceDataId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
