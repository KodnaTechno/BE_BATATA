using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema.Properties;
using Module.Seeding.ModuleDefinitions;

namespace Module.Seeding
{
    public static class ModuleSeeder
    {
        private static readonly IModuleDefinition[] ModuleDefinitions =
        {
            new TaskModuleDefinition(),
        };

        public static void SeedModules(this ModelBuilder modelBuilder)
        {
            foreach (var definition in ModuleDefinitions)
            {
                modelBuilder.Entity<Domain.Schema.Module>().HasData(definition.GetModule());
                modelBuilder.Entity<Property>().HasData(definition.GetProperties());
            }
        }
    }
}
