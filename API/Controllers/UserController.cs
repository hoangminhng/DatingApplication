using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<IEnumerable<AppUser>> GetUsers()
    {
        var users = context.Users.ToList();
        return users;
    }

    [HttpGet("{id}")] //api/user/2
    public ActionResult<AppUser> GetUser(int id)
    {
        return context.Users.Find(id);
    }

}
