using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;

namespace Module.Configurations
{
    public class PropertyConnectionConfiguration : IEntityTypeConfiguration<PropertyConnection>
    {
        public void Configure(EntityTypeBuilder<PropertyConnection> builder)
        {
            builder.HasKey(e => new { e.SourcePropertyId, e.TargetPropertyId });

            builder.Property(e => e.ConnectionType).HasConversion<string>();

            builder.HasOne(e => e.TargetProperty)
                .WithMany(p => p.TargetPropertyConnections)
                .HasForeignKey(e => e.TargetPropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.SourceProperty)
                .WithMany(p => p.SourcePropertyConnections)
                .HasForeignKey(e => e.SourcePropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
