using System.Data.Entity;
using PpApi.Models;

namespace PpApi.Database
{
    public class PpDbContext : DbContext
    {
        public PpDbContext() : base("DefaultConnection")
        { }

        public DbSet<Location> Locations { set; get; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>().HasKey(x => x.Id);
            modelBuilder.Entity<Location>().ToTable("pp_location");
        }
    }
}