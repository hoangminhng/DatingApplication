using API.Entities;

namespace API;

public interface ITokenServices
{
    string CreateToken(AppUser appUser);
}
