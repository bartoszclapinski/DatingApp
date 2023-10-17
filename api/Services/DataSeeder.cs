using API.Data;
using API.Entities;

namespace API.Services;

public class DataSeeder
{
    private readonly AppDbContext _context;
    
    public DataSeeder(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Seed()
    {
        if (!_context.Database.CanConnect()) return;
        
        if (!_context.Users.Any())
        {
            var users = GetUsers();
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }
    }
    
    private IEnumerable<AppUser> GetUsers()
    {
        var users = new List<AppUser>
        {
            new() { UserName = "user1" },
            new() { UserName = "user2" },
            new() { UserName = "user3" }
        };
        return users;
    }
}