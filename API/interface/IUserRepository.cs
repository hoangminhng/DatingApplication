using API.Entities;

namespace API;

public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUserAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams);
    Task<MemberDTO> GetMemberAsync(string username);
    Task<string> UserGender(string username);
}
