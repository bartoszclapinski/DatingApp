using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : IdentityDbContext
                <AppUser, AppRole, Guid, IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>,
                IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    // Options are set in Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    public DbSet<UserLike> Likes { get; set; }
    
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AppUser>()
                        .HasMany(ur => ur.UserRoles)
                        .WithOne(u => u.User)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();

        modelBuilder.Entity<AppRole>()
                        .HasMany(ur => ur.UserRoles)
                        .WithOne(u => u.Role)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();
        
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

        modelBuilder.Entity<Message>()
                        .HasOne(u => u.Recipient)
                        .WithMany(m => m.MessagesReceived)
                        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
                        .HasOne(u => u.Sender)
                        .WithMany(m => m.MessagesSent)
                        .OnDelete(DeleteBehavior.Restrict);

    }
}