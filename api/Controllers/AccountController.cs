using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(AppDbContext context, ITokenService tokenService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }

    [HttpPost("register")] // POST api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
        
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName = registerDto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };                
        
    }

    [HttpPost("login")] // POST api/account/login
    public async Task<ActionResult<UserDto>> Login (LoginDto loginDto)
    {
        var user = await _context.Users.Include(appUser => appUser.Photos).SingleOrDefaultAsync(u => u.UserName == loginDto.UserName.ToLower());
        if (user == null) return Unauthorized("Invalid username");
        
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any())
        {
            return Unauthorized("Invalid password");
        }
        
        return new UserDto
        { 
            UserName = user.UserName, 
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
        };
    }
    
    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}