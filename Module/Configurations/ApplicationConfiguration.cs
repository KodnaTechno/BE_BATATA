using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Configurations
{
    public class ApplicationConfiguration : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.HasMany(e => e.Workspaces)
               .WithOne(e => e.Application)
               .HasForeignKey(e => e.ApplicationId)
               .IsRequired(false); 

            builder.HasMany(e => e.Modules)
                   .WithOne(e => e.Application)
                   .HasForeignKey(e => e.ApplicationId)
                   .IsRequired(false); 
        }
    }

}
