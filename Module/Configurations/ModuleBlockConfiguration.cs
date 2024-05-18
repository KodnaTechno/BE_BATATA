using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class ModuleBlockConfiguration : IEntityTypeConfiguration<ModuleBlock>
    {
        public void Configure(EntityTypeBuilder<ModuleBlock> builder)
        {
            builder.HasMany(e => e.ModuleBlockModules).WithOne(e => e.ModuleBlock).HasForeignKey(e => e.ModuleBlockId);
        }
    }

}
