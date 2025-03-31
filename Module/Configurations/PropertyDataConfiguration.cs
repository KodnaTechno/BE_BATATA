using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Data;

namespace Module.Configurations
{
    public class PropertyDataConfiguration : IEntityTypeConfiguration<PropertyData>
    {
        public void Configure(EntityTypeBuilder<PropertyData> builder)
        {
            builder.HasOne(e => e.Property).WithMany().HasForeignKey(e => e.PropertyId);

            builder.Property(e => e.ViewType).HasConversion<string>();
            builder.Property(e => e.DataType).HasConversion<string>();
        }
    }

}
