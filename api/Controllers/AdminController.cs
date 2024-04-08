using System.Runtime.InteropServices;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;

    public AdminController(
                    UserManager<AppUser> userManager,
                    IUnitOfWork unitOfWork,
                    IPhotoService photoService)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
                        .OrderBy(u => u.UserName)
                        .Select(u => new
                        {
                            u.Id,
                            u.UserName,
                            Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                        })
                        .ToListAsync();              
                        
        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
    { 
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");
        var selectedRoles = roles.Split(",").ToArray(); 
        
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound("Could not find user");
        
        var userRoles = await _userManager.GetRolesAsync(user);
        
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");
        
        return Ok(await _userManager.GetRolesAsync(user));
    }
    
    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {
        var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(Guid photoId)
    {
        //  Get photo by id
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        
        //  If photo is not found, return not found
        if (photo == null) return NotFound("Could not find photo");
        
        //  Approve photo
        photo.IsApproved = true;

        //  Get user by photo id
        var user = await _unitOfWork.UserRepository.GetUserByPhotoId(photoId);
        
        //   If user has no main photo, set this photo as main
        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;
        
        //  Save changes
        await _unitOfWork.Complete();
        
        //  Return ok
        return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(Guid photoId)
    {
        //  Get photo by id
        var photo = await _unitOfWork.PhotoRepository.GetPhotoById(photoId);
        
        //  If photo is not found, return not found
        if (photo == null) return NotFound("Could not find photo");
        
        //  Check if photo has public id
        if (photo.PublicId != null)
        {
            //  Delete photo from cloudinary
            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            
            // If error, return bad request
            if (result.Error != null) return BadRequest(result.Error.Message);
            
            //  Remove photo from database
            if (result.Result == "ok") _unitOfWork.PhotoRepository.RemovePhoto(photo); 
        }
        else
        {
            //  Remove photo from database
            _unitOfWork.PhotoRepository.RemovePhoto(photo);
        }
        
        //  Save changes
        await _unitOfWork.Complete();
        
        //  Return ok
        return Ok();
    }
}