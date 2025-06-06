﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AppCommon.GlobalHelpers;

namespace Module.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Domain.Schema.Module>
    {
        public void Configure(EntityTypeBuilder<Domain.Schema.Module> builder)
        {
            builder.Property(e => e.Type).HasConversion<string>();

            builder.HasOne(e => e.Application)
                .WithMany(e => e.Modules)
                .HasForeignKey(e => e.ApplicationId)
                .IsRequired(false);

            builder.Property(e => e.Title)
              .HasJsonConversion();

            builder.Property(e => e.Details)
                .HasJsonConversion();

            builder.HasMany(e => e.WorkspaceModules).WithOne(e => e.Module).HasForeignKey(e => e.ModuleId);
        }
    }

}
