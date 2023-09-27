﻿using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class BuggyController : BaseApiControllers
{
    private readonly DataContext context;
    public BuggyController(DataContext context)
    {
        this.context = context;
    }

    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = context.Users.Find(-1);

        if (thing == null)
        {
            return NotFound();
        }
        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = context.Users.Find(-1);
        var thingToReturn = thing.ToString();
        return thingToReturn;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {        
        return BadRequest("This is not a good request");
    }
}