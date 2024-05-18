using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class WorkspaceConnectionDataConfiguration : IEntityTypeConfiguration<WorkspaceConnectionData>
    {
        public void Configure(EntityTypeBuilder<WorkspaceConnectionData> builder)
        {
            builder.HasOne(e => e.SourceWorkspaceData).WithMany().HasForeignKey(e => e.SourceWorkspaceDataId);
            builder.HasOne(e => e.TargetWorkspaceData).WithMany().HasForeignKey(e => e.TargetWorkspaceDataId);
            builder.HasOne(e => e.WorkspaceConnection).WithMany().HasForeignKey(e => e.WorkspaceConnectionId);
        }
    }

}
