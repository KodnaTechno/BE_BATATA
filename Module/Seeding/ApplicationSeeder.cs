using AppCommon.DTOs;
using AppCommon.GlobalHelpers;
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
                Id = SystemApplicationConstants.ProjectManagementApplicationId,
                Title = new TranslatableValue { En = "Asset Management", Ar = "إداره المرافق" }.AsText(),
                Icon = "fa-solid fa-warehouse",
                Description = new TranslatableValue
                {
                    En = "Manage all your organizational assets and their details.",
                    Ar = "إدارة جميع أصول مؤسستك وتفاصيلها"
                }.AsText(),
                CreatedAt = seedDate,
                CreatedBy = systemUserId
            });
        }
    }
}
