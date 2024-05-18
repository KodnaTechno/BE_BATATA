using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;

namespace Module.Configurations
{
    public class PropertyFormulaConfiguration : IEntityTypeConfiguration<PropertyFormula>
    {
        public void Configure(EntityTypeBuilder<PropertyFormula> builder)
        {
            builder.HasOne(e => e.Property).WithMany(e => e.PropertyFormulas).HasForeignKey(e => e.PropertyId);
        }
    }

}
