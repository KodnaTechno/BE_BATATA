using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Module.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.BusinessDomain.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.BusinessDomain.Task> builder)
        {
            builder.HasOne(e => e.ModuleData).WithOne();
        }
    }

}
