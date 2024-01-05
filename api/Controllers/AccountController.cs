using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost("register")] // POST api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
        
        var user = _mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.UserName.ToLower();
        
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        
        var rolesResult = await _userManager.AddToRoleAsync(user, "Member");
        
        if (!rolesResult.Succeeded) return BadRequest(rolesResult.Errors);

        return new UserDto
        {
            UserName = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };                
        
    }

    [HttpPost("login")] // POST api/account/login
    public async Task<ActionResult<UserDto>> Login (LoginDto loginDto)
    {

        var user = await _userManager.Users.
                        Include(p => p.Photos).
                        SingleOrDefaultAsync(u => u.UserName == loginDto.UserName.ToLower());
        
        
        if (user == null) return Unauthorized("Invalid username");
        
        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!result) return Unauthorized("Invalid password");
        
        return new UserDto
        {
            UserName = user.UserName, 
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }
    
    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}