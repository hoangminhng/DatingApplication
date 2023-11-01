using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[Authorize]
public class LikesController : BaseApiControllers
{
    private readonly IUnitOfwork _unitOfwork;
    public LikesController(IUnitOfwork unitOfwork)
    {
        this._unitOfwork = unitOfwork;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username)
    {
        var sourceUserId = User.getUserId();
        var likedUser = await _unitOfwork.UserRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _unitOfwork.LikesRepository.GetUserWithLikes(sourceUserId);

        if (likedUser == null)
        {
            return BadRequest();
        }

        if (sourceUser.UserName == username)
        {
            return BadRequest("You can not like yoursefl");
        }

        var userLike = await _unitOfwork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

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

        if (await _unitOfwork.Complete())
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
        var users = await _unitOfwork.LikesRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages));
        return Ok(users);
    }
}
