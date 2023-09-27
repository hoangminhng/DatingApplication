using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UserController : BaseApiControllers
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();

        return Ok(users);
    }

    // [HttpGet("{id}")] //api/user/2
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     return await userRepository.GetUserByIdAsync(id);
    // }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUserByName(string username){
        return await userRepository.GetMemberAsync(username);
    }
}
