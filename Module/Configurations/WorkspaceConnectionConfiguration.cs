using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class WorkspaceConnectionConfiguration : IEntityTypeConfiguration<WorkspaceConnection>
    {
        public void Configure(EntityTypeBuilder<WorkspaceConnection> builder)
        {
            builder.HasOne(w => w.SourceWorkspace)
               .WithMany()
               .HasForeignKey(w => w.SourceWorkspaceId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(w => w.TargetWorkspace)
                .WithMany()
                .HasForeignKey(w => w.TargetWorkspaceId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
