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
        // watch_lists_medias
        modelBuilder.Entity<WatchList>()
            .HasMany(w => w.Medias)
            .WithMany(m => m.WatchLists)
            .UsingEntity(j => j.ToTable("watch_lists_medias"));
        // medias_reviews
        modelBuilder.Entity<Review>()
            .HasMany(r => r.Medias)
            .WithMany(m => m.Reviews)
            .UsingEntity(j => j.ToTable("medias_reviews"));
        // medias_genres
        modelBuilder.Entity<Genre>()
            .HasMany(g => g.Medias)
            .WithMany(m => m.Genres)
            .UsingEntity(j => j.ToTable("medias_genres"));
        // medias_persons_roles (ternary join)
        modelBuilder.Entity<MediaPersonRole>()
            .HasOne(mpr => mpr.Media)
            .WithMany(m => m.MediaPersonRoles)
            .HasForeignKey(mpr => mpr.MediaId);
        modelBuilder.Entity<MediaPersonRole>()
            .HasOne(mpr => mpr.Person)
            .WithMany(p => p.MediaPersonRoles)
            .HasForeignKey(mpr => mpr.PersonId);
        modelBuilder.Entity<MediaPersonRole>()
            .HasOne(mpr => mpr.Role)
            .WithMany(r => r.MediaPersonRoles)
            .HasForeignKey(mpr => mpr.RoleId);
        modelBuilder.Entity<MediaPersonRole>()
            .HasKey(mpr => new { mpr.MediaId, mpr.PersonId, mpr.RoleId });
    }
    
    public DbSet<Privilege> Privileges { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<WatchList> WatchLists { get; set; }
    public DbSet<Media> Medias { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<MediaPersonRole> MediaPersonRoles { get; set; }
}
