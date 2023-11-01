using API.Data;
using API.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class UserRepository : IUserRepository
{
    private readonly DataContext context;

    private readonly IMapper mapper;
    public UserRepository(DataContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<MemberDTO> GetMemberAsync(string username)
    {
        return await context.Users
                    .Where(x => x.UserName == username)
                    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync();
    }

    public async Task<string> UserGender(string username)
    {
        return await context.Users.Where(x => x.UserName == username)
            .Select(x => x.Gender).FirstOrDefaultAsync();
    }

    public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = context.Users.AsQueryable();

        query = query.Where(u => u.UserName != userParams.CurrentUsername);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.Created),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        return await PageList<MemberDTO>.CreateAsync(
            query.AsNoTracking().ProjectTo<MemberDTO>(mapper.ConfigurationProvider),
            userParams.PageNumber,
            userParams.PageSize);
    }

    public async Task<IEnumerable<AppUser>> GetUserAsync()
    {
        return await context.Users
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
