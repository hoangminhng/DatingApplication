using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });
        services.AddCors(); //to allow many recourses from different origin communicate with
        services.AddScoped<ITokenServices, TokenServices>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySetting>(configuration.GetSection("CloudinarySettings"));
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();
        services.AddScoped<ILikeRepository, LikesRepository>();
        services.AddScoped<IMessagesRepository, MessagesRepository>();
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();
        return services;
    }
}
