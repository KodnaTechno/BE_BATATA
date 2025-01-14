using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowStepDataConfiguration : IEntityTypeConfiguration<WorkflowStepData>
    {
        public void Configure(EntityTypeBuilder<WorkflowStepData> builder)
        {
            // Table configuration
            builder.ToTable("WorkflowStepInstances");
            builder.HasKey(x => x.Id);

            // JSON conversions
            builder.Property(e => e.InputData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<JsonDocument>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.OutputData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<JsonDocument>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.StepVariables)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default));

            // Relationships
            builder.HasOne<WorkflowData>()
                .WithMany(w => w.StepInstances)
                .HasForeignKey(s => s.WorkflowDataId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => new { e.WorkflowDataId, e.StepId })
                .HasDatabaseName("IX_WorkflowStepInstances_WorkflowInstance_Step");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_WorkflowStepInstances_Status");
        }
    }
}
