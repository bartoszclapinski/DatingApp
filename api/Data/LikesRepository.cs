using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly AppDbContext _context;

    public LikesRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<UserLike> GetUserLike(Guid sourceUserId, Guid likedUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, likedUserId);
    }

    public async Task<AppUser> GetUserWithLikes(Guid userId)
    {
        return await _context.Users
                        .Include(x => x.LikedUsers)
                        .FirstOrDefaultAsync(x => x.Id == userId);       
    }

    public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, Guid userId)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        switch (predicate)
        {
            case "liked":
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);
                break;
            case "likedBy":
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
                break;
        }

        return await users.Select(user => new LikeDto
        {
                        UserName = user.UserName,
                        KnownAs = user.KnownAs,
                        Age = user.DateOfBirth.CalculateAge(),
                        PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                        City = user.City,
                        Id = user.Id
        }).ToListAsync();
    }
}