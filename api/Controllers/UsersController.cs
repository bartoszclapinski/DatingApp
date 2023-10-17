using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase {
    
    private readonly UsersController _usersController;

    public UsersController(UsersController usersController)
    {
        _usersController = usersController ?? throw new ArgumentNullException(nameof(usersController));
    }    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _usersController.GetUsers();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(Guid id)
    {
        var user = await _usersController.GetUser(id);
        return Ok(user);
    }
}