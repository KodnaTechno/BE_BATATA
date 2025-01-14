using AppWorkflow.Core.Domain.Data;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Infrastructure.Data.Configurations
{
    public class ApprovalHistoryConfiguration : IEntityTypeConfiguration<ApprovalHistory>
    {
        public void Configure(EntityTypeBuilder<ApprovalHistory> entity)
        {
            entity.ToTable("ApprovalHistories");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.UpdatedProperties)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default));

            entity.HasIndex(e => e.ApprovalRequestId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Timestamp);
        }
    }
}
