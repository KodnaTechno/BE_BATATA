
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppWorkflow.Domain.Schema;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class StepTransitionConfiguration : IEntityTypeConfiguration<StepTransition>
    {
        public void Configure(EntityTypeBuilder<StepTransition> builder)
        {
            builder.ToTable("StepTransitions");
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            // Add indexes
            builder.HasIndex(e => new { e.SourceStepId, e.TargetStepId})
                .HasDatabaseName("IX_StepTransitions_FromTo");
            
        }
    }
}
