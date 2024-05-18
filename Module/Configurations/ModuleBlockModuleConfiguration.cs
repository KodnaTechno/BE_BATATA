using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class ModuleBlockModuleConfiguration : IEntityTypeConfiguration<ModuleBlockModule>
    {
        public void Configure(EntityTypeBuilder<ModuleBlockModule> builder)
        {
            builder.HasOne(e => e.Module).WithMany().HasForeignKey(e => e.ModuleId);
            builder.HasOne(e => e.ModuleBlock).WithMany(e => e.ModuleBlockModules).HasForeignKey(e => e.ModuleBlockId);
        }
    }

}
