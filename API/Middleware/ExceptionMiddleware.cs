using System.Net;

namespace API;

public class ExceptionMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionMiddleware> logger;
    private readonly IHostEnvironment environment;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment environment)
    {
        this.next = next;
        this.logger = logger;
        this.environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response  = environment.IsDevelopment();
        }
    }
}
