using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
    {
        public void Configure(EntityTypeBuilder<Workspace> builder)
        {
            builder.Property(e => e.Type).HasConversion<string>();
            builder.HasMany(e => e.WorkspaceModules).WithOne(e => e.Workspace).HasForeignKey(e => e.WorkspaceId);
            builder.HasMany(e => e.WorkspaceModuleBlocks).WithOne(e => e.Workspace).HasForeignKey(e => e.WorkspaceId);
            builder.HasOne(e => e.Application).WithMany(e => e.Workspaces).HasForeignKey(e => e.ApplicationId);
        }
    }

}
