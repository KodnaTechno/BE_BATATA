using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class WorkspaceModuleBlockConfiguration : IEntityTypeConfiguration<WorkspaceModuleBlock>
    {
        public void Configure(EntityTypeBuilder<WorkspaceModuleBlock> builder)
        {
            builder.HasOne(e => e.Workspace).WithMany(e => e.WorkspaceModuleBlocks).HasForeignKey(e => e.WorkspaceId);
            builder.HasOne(e => e.Module).WithMany().HasForeignKey(e => e.ModuleBlockId);
        }
    }

}
