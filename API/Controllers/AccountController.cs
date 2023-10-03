using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

public class AccountController : BaseApiControllers
{
    private DataContext context;
    private ITokenServices tokenServices;
    private IMapper mapper;
    public AccountController(DataContext context, ITokenServices tokenServices, IMapper mapper)
    {
        this.context = context;
        this.tokenServices = tokenServices;
        this.mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTOs>> Register(RegisterDTO registerDTO)
    {

        if (await UserExists(registerDTO.Username))
        {
            return BadRequest("Username is already existed");
        }

        var user = mapper.Map<AppUser>(registerDTO);

        using var hmac = new HMACSHA512();


        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new UserDTOs
        {
            Username = user.UserName,
            Token = tokenServices.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTOs>> Login(LoginDTO loginDTO)
    {
        var user = await context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);
        if (user == null) return Unauthorized("Invalide username");

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
            Token = tokenServices.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender,
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
