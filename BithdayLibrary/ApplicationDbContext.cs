using Microsoft.EntityFrameworkCore;
using BithdayLibrary.Services;

namespace BithdayLibrary
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseServise>().ToTable("Services");
            modelBuilder.Entity<VKService>().ToTable("VKService");
            modelBuilder.Entity<TelegramService>().ToTable("TelegramService");
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Services)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(
                _connectionString,
                new MySqlServerVersion(new System.Version(8, 0, 27))
            );
            options.UseLazyLoadingProxies();
        }
    }
}
