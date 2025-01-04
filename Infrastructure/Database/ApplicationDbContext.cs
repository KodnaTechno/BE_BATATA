using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Database.Domain;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Database.Configration;

namespace Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AppConfig> AppConfigs { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AppConfigConfigration());
            modelBuilder.ApplyConfiguration(new EventLogConfigration());
        }
    }
}
