using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    public Task<UserLike> GetUserLike(Guid sourceUserId, Guid likedUserId)
    {
        throw new NotImplementedException();
    }

    public Task<AppUser> GetUserWithLikes(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, Guid userId)
    {
        throw new NotImplementedException();
    }
}