using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class ModuleDataConfiguration : IEntityTypeConfiguration<ModuleData>
    {
        public void Configure(EntityTypeBuilder<ModuleData> builder)
        {
            builder.HasOne(e => e.Module).WithMany().HasForeignKey(e => e.ModuleId);
        }
    }

}
