using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppWorkflow.Core.Domain.Data.WorkflowStepData;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class StepExecutionLogConfiguration : IEntityTypeConfiguration<StepExecutionLog>
    {
        public void Configure(EntityTypeBuilder<StepExecutionLog> builder)
        {
            builder.ToTable("StepExecutionLogs");
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Data)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

        }
    }
}
