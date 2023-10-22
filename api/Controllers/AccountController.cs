using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpPost("register")] // POST api/account/register
    public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
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

        return user;
    }

    [HttpPost("login")] // POST api/account/login
    public async Task<ActionResult<AppUser>> Login (LoginDto loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.UserName.ToLower());
        if (user == null) return Unauthorized("Invalid username");
        
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any())
        {
            return Unauthorized("Invalid password");
        }
        
        return user;
    }
    
    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}