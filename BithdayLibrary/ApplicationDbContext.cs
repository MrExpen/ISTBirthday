using System;
using Microsoft.EntityFrameworkCore;
using BirthdayLibrary.Services;

namespace BirthdayLibrary
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext()
        {
            
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseMySql(Environment.GetEnvironmentVariable("CONNECTION_STRING"), new MySqlServerVersion("8.0"),
                        optionsBuilder => optionsBuilder.EnableRetryOnFailure()).UseLazyLoadingProxies();
            }
            base.OnConfiguring(optionsBuilder);
        }
    }
}
