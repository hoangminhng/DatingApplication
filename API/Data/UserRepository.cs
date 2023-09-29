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

    public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query =  context.Users
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            .AsNoTracking();

        return await PageList<MemberDTO>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
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

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        context.Entry(user).State = EntityState.Modified;
    }
}
