using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.Models;
using AppWorkflow.Infrastructure.Services.Actions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class ApprovalConfiguration : IEntityTypeConfiguration<ApprovalRequest>
    {
        public void Configure(EntityTypeBuilder<ApprovalRequest> entity)
        {
            entity.ToTable("ApprovalRequests");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ApproverIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default));

            entity.Property(e => e.EditableProperties)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<ApprovalPropertyConfig>>(v, JsonSerializerOptions.Default));

            entity.Property(e => e.ModuleData)
                 .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<WorkflowModuleData>(v, JsonSerializerOptions.Default));


            entity.Property(e => e.UpdatedProperties)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string,object>>(v, JsonSerializerOptions.Default));

            entity.HasIndex(e => e.WorkflowDataId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ExpiresAt);
        }
    }
}
