using Microsoft.EntityFrameworkCore;

namespace FileStorge.Providers.Database
{
    public class FileDbContext : DbContext
    {

        public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
        {
        }

        public DbSet<File> Files { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            FileSchema(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void FileSchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>().HasKey(f => f.FileId);

            modelBuilder.Entity<File>().Property(f => f.Name).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.Binary).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.Extension).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.ContentType).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.CreatedAt).IsRequired();
            modelBuilder.Entity<File>().Property(f => f.Owner).IsRequired();
        }
    }
}