using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Module.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Domain.Schema.Module>
    {
        public void Configure(EntityTypeBuilder<Domain.Schema.Module> builder)
        {
            builder.Property(e => e.Type).HasConversion<string>();
            builder.HasMany(e => e.WorkspaceModules).WithOne(e => e.Module).HasForeignKey(e => e.ModuleId);
        }
    }

}
