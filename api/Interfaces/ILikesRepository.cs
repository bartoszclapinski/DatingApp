using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(Guid sourceUserId, Guid likedUserId);
    Task<AppUser> GetUserWithLikes(Guid userId);
    Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, Guid userId);
}