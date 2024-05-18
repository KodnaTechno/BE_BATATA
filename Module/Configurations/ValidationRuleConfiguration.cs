using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;

namespace Module.Configurations
{
    public class ValidationRuleConfiguration : IEntityTypeConfiguration<ValidationRule>
    {
        public void Configure(EntityTypeBuilder<ValidationRule> builder)
        {
            builder.Property(e => e.RuleType).HasConversion<string>();
            builder.HasOne(e => e.Property).WithMany(e => e.ValidationRules).HasForeignKey(e => e.PropertyId);
        }
    }

}
