using Microsoft.EntityFrameworkCore;
using TeleMed.Models;

namespace TeleMed.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {

        }
       
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can configure relationships, indexes, etc., here if needed
        }
    }
}
