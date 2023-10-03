using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[Authorize]
public class LikesController : BaseApiControllers
{
    private readonly IUserRepository userRepository;
    private readonly ILikeRepository likeRepository;
    public LikesController(IUserRepository userRepository, ILikeRepository likeRepository)
    {
        this.userRepository = userRepository;
        this.likeRepository = likeRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.getUserId();
        var likedUser = await userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await likeRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null)
        {
            return BadRequest();
        }

        if (sourceUser.UserName == username)
        {
            return BadRequest("You can not like yoursefl");
        }

        var userLike = await likeRepository.GetUserLike(sourceUserId, likedUser.Id);

        if (userLike != null)
        {
            return BadRequest("You already  like this user");
        }

        userLike = new UserLike
        {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if (await userRepository.SaveAllAsync())
        {
            return Ok();
        }
        return BadRequest("Fail to like user");
    }

    [HttpGet]
    public async Task<ActionResult<PageList<LikeDTO>>> GetUserLikes([FromQuery]LikeParams likeParams)
    {
        likeParams.UserId = User.getUserId();
        int userId = User.getUserId();
        var users = await likeRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));
        return Ok(users);
    }
}
