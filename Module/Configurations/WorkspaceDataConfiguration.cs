using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class WorkspaceDataConfiguration : IEntityTypeConfiguration<WorkspaceData>
    {
        public void Configure(EntityTypeBuilder<WorkspaceData> builder)
        {
            builder.HasMany(e => e.WorkspaceConnections).WithOne(e => e.SourceWorkspaceData).HasForeignKey(e => e.SourceWorkspaceDataId);
        }
    }

}
