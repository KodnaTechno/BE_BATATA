using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class WorkflowRelationConfiguration : IEntityTypeConfiguration<WorkflowRelation>
    {
        public void Configure(EntityTypeBuilder<WorkflowRelation> builder)
        {
            // Table configuration
            builder.ToTable("WorkflowRelations");
            builder.HasKey(x => x.Id);

            // Property configurations
            builder.Property(e => e.Name)
                .HasMaxLength(200);

            // JSON conversions
            builder.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default));

            // Relationships
            builder.HasOne(e => e.ParentInstance)
                .WithMany(w => w.ChildRelations)
                .HasForeignKey(e => e.ParentInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ChildInstance)
                .WithMany(w => w.ParentRelations)
                .HasForeignKey(e => e.ChildInstanceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => new { e.ParentInstanceId, e.RelationType })
                .HasDatabaseName("IX_WorkflowRelations_Parent");

            builder.HasIndex(e => new { e.ChildInstanceId, e.RelationType })
                .HasDatabaseName("IX_WorkflowRelations_Child");
        }
    }
}
