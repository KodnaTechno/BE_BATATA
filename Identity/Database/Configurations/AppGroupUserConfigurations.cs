using AppIdentity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppIdentity.Database.Configurations;

public class AppGroupUserConfigurations: IEntityTypeConfiguration<AppGroupUser>
{
    public void Configure(EntityTypeBuilder<AppGroupUser> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.Group)
            .WithMany(x => x.GroupUsers)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.ToTable("appGroupUsers");
    }
}