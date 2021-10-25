using Microsoft.EntityFrameworkCore;

namespace BithdayLibrary
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySql("server=127.0.0.1;user=mrexpen;password=m{V8[W?THnf@GHVckkv3'7=Rbm/2P=QC._L8br*^Dk;database=ISTBirthdays;", new MySqlServerVersion(new System.Version(8, 0, 26)));
    }
}
