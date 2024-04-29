using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeleMed.Models;

namespace TeleMed.Data.Abstracts;

public interface IAppDbContext
{
    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Providers> Providers { get; set; }
    public DbSet<Patients> Patients { get; set; }
    public DbSet<Appointments> Appointments { get; set; }
    
    public int SaveChanges();
    
    ChangeTracker ChangeTracker { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    EntityEntry Entry(object entity);
}