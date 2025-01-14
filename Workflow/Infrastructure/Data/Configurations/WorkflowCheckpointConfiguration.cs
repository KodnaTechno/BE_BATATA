using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowCheckpointConfiguration : IEntityTypeConfiguration<WorkflowCheckpoint>
    {
        public void Configure(EntityTypeBuilder<WorkflowCheckpoint> builder)
        {
            // Table configuration
            builder.ToTable("WorkflowCheckpoints");
            builder.HasKey(x => x.Id);

            // JSON conversions
            builder.Property(e => e.Variables)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.StepData)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<Guid, Dictionary<string, object>>>(v, JsonSerializerOptions.Default));

            // Indexes
            builder.HasIndex(e => e.InstanceId)
                .HasDatabaseName("IX_WorkflowCheckpoints_InstanceId");

            builder.HasIndex(e => e.CheckpointTime)
                .HasDatabaseName("IX_WorkflowCheckpoints_CheckpointTime");
        }
    }
}
