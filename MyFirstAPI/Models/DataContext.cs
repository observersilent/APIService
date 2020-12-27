using Microsoft.EntityFrameworkCore;

namespace MyFirstAPI.Models
{
    public class DataContext : DbContext
    {
        public DbSet<Data> DataTable { get; set; }
        public DbSet<CurveTable> Curves { get; set; }
        public DbSet<PointTable> Points { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Database.sqlite3");
        }

    }
}
