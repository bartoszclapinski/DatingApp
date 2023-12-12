using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(Guid sourceUserId, Guid likedUserId);
    Task<AppUser> GetUserWithLikes(Guid userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
}