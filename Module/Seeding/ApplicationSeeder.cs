using Microsoft.EntityFrameworkCore;
using Module.Domain.Schema;

namespace Module.Seeding
{
    public static class ApplicationSeedExtensions
    {
        public static void SeedApplications(this ModelBuilder modelBuilder)
        {
            var systemUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var seedDate = new DateTime(2024, 1, 1);

            modelBuilder.Entity<Application>().HasData(new Application
            {
                Id = SystemApplicationConstants.AssetManagementApplicationId,
                Title = "Asset Management",
                Icon = "fa-solid fa-warehouse",
                Description = "Manage all your organizational assets and their details.",
                CreatedAt = seedDate,
                CreatedBy = systemUserId
            });
        }
    }
}
