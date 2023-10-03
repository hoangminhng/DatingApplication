using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikesRepository : ILikeRepository
{
    private readonly DataContext dataContext;
    public LikesRepository(DataContext dataContext)
    {
        this.dataContext = dataContext;
    }
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await dataContext.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PageList<LikeDTO>> GetUserLikes(LikeParams likeParams)
    {
        var users = dataContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = dataContext.Likes.AsQueryable();

        if (likeParams.Predicate == "liked")
        {
            likes = likes.Where(like =>  like.SourceUserId == likeParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        if (likeParams.Predicate == "likedBy")
        {
            likes = likes.Where(like =>  like.TargetUserId == likeParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUser = users.Select(user => new LikeDTO
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalcualateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id,
        });

        return await PageList<LikeDTO>.CreateAsync(likedUser, likeParams.PageNumber, likeParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await dataContext.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
