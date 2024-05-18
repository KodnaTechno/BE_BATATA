﻿using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;
using Module.Domain.Schema;
using Module.Domain.Data;
using Module.Configurations;

namespace Module
{
    public class ModuleDbContext : DbContext
    {
        #region Schema
        public DbSet<Workspace> Workspaces { get; set; }
        public DbSet<Domain.Schema.Module> Modules { get; set; }
        public DbSet<ModuleBlock> ModuleBlocks { get; set; }
        public DbSet<WorkspaceModule> WorkspaceModules { get; set; }
        public DbSet<WorkspaceConnection> WorkspaceConnections { get; set; }
        public DbSet<ModuleBlockModule> ModuleBlockModules { get; set; }
        public DbSet<WorkspaceModuleBlock> WorkspaceModuleBlocks { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyConnection> PropertyConnections{ get; set; }
        public DbSet<ValidationRule> ValidationRules { get; set; }
        public DbSet<PropertyFormula> PropertyFormulas { get; set; }
        #endregion

        #region SchemaData
        public DbSet<ModuleData> ModuleData { get; set; }
        public DbSet<Workspace> WorkspaceData { get; set; }
        public DbSet<WorkspaceConnection> WorkspaceConnectionData { get; set; }
        public DbSet<Property> PropertyData { get; set; }

        #endregion

        public ModuleDbContext(DbContextOptions<ModuleDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("module");
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyConnectionConfiguration());
            modelBuilder.ApplyConfiguration(new ValidationRuleConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceModuleConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleDataConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyDataConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceConnectionDataConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceDataConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleBlockConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleBlockModuleConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceConnectionConfiguration());
            modelBuilder.ApplyConfiguration(new WorkspaceModuleBlockConfiguration());
            modelBuilder.ApplyConfiguration(new PropertyFormulaConfiguration());
        }
    }
}
