﻿using API.Entities;

namespace API;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<AppUser>> GetUserAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<IEnumerable<MemberDTO>> GetMembersAsync();
    Task<MemberDTO> GetMemberAsync(string username);
}
