using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController {
    
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetUsersAsync();
        var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

        return Ok(usersToReturn);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MemberDto>> GetUser(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        var userToReturn = _mapper.Map<MemberDto>(user);
        return Ok(userToReturn);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await _userRepository.GetUserByUserNameAsync(username);
        var userToReturn = _mapper.Map<MemberDto>(user);
        return Ok(userToReturn);
    }
    
}