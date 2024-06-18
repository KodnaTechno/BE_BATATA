using System.Collections.Generic;
using System;
using Infrastructure.Database.Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<AppConfig> AppConfigs { get; set; }
    }
}
