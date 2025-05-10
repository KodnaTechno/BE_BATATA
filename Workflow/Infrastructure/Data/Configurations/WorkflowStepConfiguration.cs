using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowStepConfiguration : IEntityTypeConfiguration<WorkflowStep>
    {
        public void Configure(EntityTypeBuilder<WorkflowStep> builder)
        {
            // Table configuration
            builder.ToTable("WorkflowSteps");
            builder.HasKey(x => x.Id);

            // Property configurations
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.ActionType)
                .IsRequired()
                .HasMaxLength(100);

            // JSON conversions
            builder.Property(e => e.ActionConfiguration)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<JsonDocument>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.RetryPolicy)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<RetryPolicy>(v, JsonSerializerOptions.Default));

            // Relationships
            builder.HasOne<Workflow>()
                .WithMany(w => w.Steps)
                .HasForeignKey(s => s.WorkflowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.WorkflowId)
                .HasDatabaseName("IX_WorkflowSteps_WorkflowId");

            builder.HasIndex(e => e.ActionType)
                .HasDatabaseName("IX_WorkflowSteps_ActionType");
        }
    }

}
