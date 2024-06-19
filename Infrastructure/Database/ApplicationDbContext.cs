using System.Collections.Generic;
using System;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new AppConfigConfigration());
        }
    }
}
