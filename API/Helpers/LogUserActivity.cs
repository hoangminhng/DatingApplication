﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace API;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
        {
            return;
        }

        var username = resultContext.HttpContext.User.getUserId();

        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await repo.GetUserByIdAsync(username);
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();
    }
}
