using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class AppDbContext : DbContext
{
    // Options are set in Program.cs
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<AppUser> Users { get; set; }
    
}