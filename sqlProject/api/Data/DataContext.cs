using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // users_privileges
        modelBuilder.Entity<Privilege>()
            .HasMany(p => p.Users)
            .WithMany(u => u.Privileges)
            .UsingEntity(j => j.ToTable("users_privileges"));
        // users_subscriptions
        modelBuilder.Entity<Subscription>()
            .HasMany(s => s.Users)
            .WithMany(u => u.Subscriptions)
            .UsingEntity(j => j.ToTable("users_subscriptions"));
    }
    
    public DbSet<Privilege> Privileges { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Profile> Profiles { get; set; }
}
