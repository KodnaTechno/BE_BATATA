using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowDataConfiguration : IEntityTypeConfiguration<WorkflowData>
    {
        public void Configure(EntityTypeBuilder<WorkflowData> builder)
        {
            // Table configuration
            builder.ToTable("WorkflowInstances");
            builder.HasKey(x => x.Id);

            // Property configurations
            builder.Property(e => e.WorkflowVersion)
                .IsRequired()
                .HasMaxLength(50);

            // JSON conversions
            builder.Property(e => e.ModuleData)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<WorkflowModuleData>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Variables)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default));

            // Indexes
            builder.HasIndex(e => e.WorkflowId)
                .HasDatabaseName("IX_WorkflowInstances_WorkflowId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_WorkflowInstances_Status");

            builder.HasIndex(e => e.StartedAt)
                .HasDatabaseName("IX_WorkflowInstances_StartedAt");

            // Query filter
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
