﻿using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(Guid id);
    Task<AppUser> GetUserByUserNameAsync(string userName);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    Task<MemberDto> GetMemberAsync(string userName, bool isCurrentUser);
    Task<string> GetUserGender(string username);
    Task<AppUser> GetUserByPhotoId(Guid photoId);
}