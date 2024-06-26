﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    
    public void Update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users
                        .Include(p => p.Photos)
                        .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUserNameAsync(string userName)
    {
        return await _context.Users
                        .Include(p => p.Photos)
                        .SingleOrDefaultAsync(u => u.UserName == userName);
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        
        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);
        
        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
        
        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
        
        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };
        
        return await PagedList<MemberDto>.CreateAsync(
                        query.AsNoTracking().ProjectTo<MemberDto>(_mapper.ConfigurationProvider), 
                        userParams.PageNumber, 
                        userParams.PageSize);
    }

    public async Task<MemberDto> GetMemberAsync(string userName, bool isCurrentUser)
    {
        var query = _context.Users
                        .Where(u => u.UserName == userName)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                        .AsQueryable();
        if (isCurrentUser) query = query.IgnoreQueryFilters();
        
        return await query.FirstOrDefaultAsync();
    }

    public async Task<string> GetUserGender(string username)
    {
        return await _context.Users
                        .Where(u => u.UserName == username)
                        .Select(g => g.Gender)
                        .FirstOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByPhotoId(Guid photoId)
    {
        return await _context.Users
                        .Include(p => p.Photos)
                        .IgnoreQueryFilters()
                        .Where(u => u.Photos.Any(p => p.Id == photoId))
                        .FirstOrDefaultAsync();
    }
}