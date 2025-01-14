using AppWorkflow.Core.Domain.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowConfiguration : IEntityTypeConfiguration<Workflow>
    {
        public void Configure(EntityTypeBuilder<Workflow> builder)
        {
            // Table configuration
            builder.ToTable("Workflows");
            builder.HasKey(x => x.Id);

            // Property configurations
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.ModuleType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Version)
                .IsRequired()
                .HasMaxLength(50);

            // JSON conversions
            builder.Property(e => e.TriggerConfigs)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<TriggerConfiguration>>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.RetryPolicy)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<RetryPolicy>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Variables)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<WorkflowVariable>>(v, JsonSerializerOptions.Default));

            // Indexes
            builder.HasIndex(e => new { e.ModuleType, e.Status })
                .HasDatabaseName("IX_Workflows_ModuleType_Status");

            builder.HasIndex(e => e.Version)
                .HasDatabaseName("IX_Workflows_Version");

            builder.HasIndex(e => e.IsLatestVersion)
                .HasDatabaseName("IX_Workflows_IsLatestVersion");

            // Query filter
            builder.HasQueryFilter(w => !w.IsDeleted);
        }
    }
}
