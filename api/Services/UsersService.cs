using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UsersService : IUsersService
{
    private readonly AppDbContext _context;

    public UsersService(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }
    
    public async Task<AppUser> GetUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        return user;
    }
}