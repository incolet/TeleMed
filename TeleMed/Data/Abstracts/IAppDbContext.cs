using Microsoft.EntityFrameworkCore;
using TeleMed.Models;

namespace TeleMed.Data.Abstracts;

public interface IAppDbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Providers> Providers { get; set; }
    public DbSet<Patients> Patients { get; set; }
    public DbSet<Appointments> Appointments { get; set; }
    
    public int SaveChanges();
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

}