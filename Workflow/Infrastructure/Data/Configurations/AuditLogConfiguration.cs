using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            // Table configuration
            builder.ToTable("AuditLogs");
            builder.HasKey(x => x.Id);

            // Property configurations
            builder.Property(e => e.EntityType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.UserId)
                .HasMaxLength(100);

            builder.Property(e => e.UserName)
                .HasMaxLength(200);

            // JSON conversions
            builder.Property(e => e.OldValues)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<JsonDocument>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.NewValues)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<JsonDocument>(v, JsonSerializerOptions.Default));

            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            // Indexes
            builder.HasIndex(e => new { e.EntityType, e.EntityId })
                .HasDatabaseName("IX_AuditLogs_Entity");

            builder.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_AuditLogs_Timestamp");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_AuditLogs_UserId");
        }
    }
}
