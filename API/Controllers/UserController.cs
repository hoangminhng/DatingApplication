using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")] // api/user
public class UserController : ControllerBase
{
    private readonly DataContext context;
    public UserController(DataContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await context.Users.ToListAsync();
        return users;
    }

    [HttpGet("{id}")] //api/user/2
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return await context.Users.FindAsync(id);
    }
}
