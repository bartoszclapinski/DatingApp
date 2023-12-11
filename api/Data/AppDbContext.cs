using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    // Options are set in Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserLike>()
                        .HasKey(k => new {k.SourceUserId, k.LikedUserId});

        modelBuilder.Entity<UserLike>()
                        .HasOne(s => s.SourceUser)
                        .WithMany(l => l.LikedUsers)
                        .HasForeignKey(s => s.SourceUserId)
                        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserLike>()
                        .HasOne(s => s.LikedUser)
                        .WithMany(l => l.LikedByUsers)
                        .HasForeignKey(k => k.LikedUserId)
                        .OnDelete(DeleteBehavior.NoAction); // For SQL Server
    }
}