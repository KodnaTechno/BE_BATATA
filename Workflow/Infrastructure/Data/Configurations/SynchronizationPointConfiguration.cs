using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class SynchronizationPointConfiguration : IEntityTypeConfiguration<SynchronizationPoint>
    {
        public void Configure(EntityTypeBuilder<SynchronizationPoint> builder)
        {
            builder.ToTable("SynchronizationPoints");
            builder.HasKey(x => x.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            // Add indexes
            builder.HasIndex(e => e.WorkflowInstanceId)
                .HasDatabaseName("IX_SynchronizationPoints_WorkflowInstanceId");

            builder.HasIndex(e => new { e.WorkflowInstanceId, e.Name })
                .HasDatabaseName("IX_SynchronizationPoints_Instance_Name")
                .IsUnique();
        }
    }
}
