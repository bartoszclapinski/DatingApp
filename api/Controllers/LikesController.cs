using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikesRepository _likesRepository;

    public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _likesRepository = likesRepository ?? throw new ArgumentNullException(nameof(likesRepository));
    }

    [HttpPost("{username}")] // POST api/likes/{username}
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.GetUserId();
        var likedUser = await _userRepository.GetUserByUserNameAsync(username);
        var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
        
        if (likedUser == null) return NotFound();
        if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");
        
        var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
        if (userLike != null) return BadRequest("You already like this user");

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            LikedUserId = likedUser.Id
        };
        
        sourceUser.LikedUsers.Add(userLike);
        
        if (await _userRepository.SaveAllAsync()) return Ok();
        return BadRequest("Failed to like user");
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        
        var users = await _likesRepository.GetUserLikes(likesParams);
        Response.AddPaginationsHeader(
                        new PaginationHeader(
                                        users.CurrentPage,
                                        users.PageSize,
                                        users.TotalCount,
                                        users.TotalPages));
        return Ok(users);
    }
}