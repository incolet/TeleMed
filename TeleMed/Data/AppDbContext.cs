using Microsoft.EntityFrameworkCore;
using TeleMed.Data.Abstracts;
using TeleMed.Models;

namespace TeleMed.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Providers> Providers { get; set; }
        public DbSet<Patients> Patients { get; set; }
        public DbSet<Appointments> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
        }

    }
}
