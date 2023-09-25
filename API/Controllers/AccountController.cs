using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiControllers
{
    private DataContext context;
    private ITokenServices tokenServices;
    public AccountController(DataContext context, ITokenServices tokenServices)
    {
        this.context = context;
        this.tokenServices = tokenServices;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTOs>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username))
        {
            return BadRequest("Username is already existed");
        }
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return new UserDTOs
        {
            Username = user.UserName,
            Token = tokenServices.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTOs>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
        if (user == null) return Unauthorized("invalide username");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i])
            {
                return Unauthorized("Invalid Password");
            }
        }

        return new UserDTOs
        {
            Username = user.UserName,
            Token = tokenServices.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
