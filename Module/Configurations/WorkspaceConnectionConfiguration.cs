using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class WorkspaceConnectionConfiguration : IEntityTypeConfiguration<WorkspaceConnection>
    {
        public void Configure(EntityTypeBuilder<WorkspaceConnection> builder)
        {
            builder.HasOne(e => e.SourceWorkspace).WithMany().HasForeignKey(e => e.SourceWorkspaceId);
            builder.HasOne(e => e.TargetWorkspace).WithMany().HasForeignKey(e => e.TargetWorkspaceId);
        }
    }

}
