using System.Security.Claims;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
[Authorize]
public class UserController : BaseApiControllers
{
    private readonly IUnitOfwork _unitOfwork;
    private readonly IMapper mapper;
    private readonly IPhotoService photoService;
    public UserController(IUnitOfwork UnitOfwork, IMapper mapper,
        IPhotoService photoService)
    {
        this._unitOfwork = UnitOfwork;
        this.mapper = mapper;
        this.photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
    {
        var gender = await _unitOfwork.UserRepository.UserGender(User.getUserName());
        userParams.CurrentUsername = User.getUserName();

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        var loggedInUser = await _unitOfwork.UserRepository.GetUserByUsernameAsync(username);

        var users = await _unitOfwork.UserRepository.GetMembersAsync(userParams);

        var userList = users.Where(x => x.Id != loggedInUser.Id).ToList();

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
                                                            users.TotalCount, users.TotalPages));
        return Ok(userList);
    }

    // [HttpGet("{id}")] //api/user/2
    // public async Task<ActionResult<AppUser>> GetUser(int id)
    // {
    //     return await userRepository.GetUserByIdAsync(id);
    // }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUserByName(string username)
    {
        return await _unitOfwork.UserRepository.GetMemberAsync(username);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _unitOfwork.UserRepository.GetUserByUsernameAsync(username);
        if (user == null)
        {
            return NotFound();
        }

        mapper.Map(memberUpdateDTO, user);
        if (await _unitOfwork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
    {
        var user = await _unitOfwork.UserRepository.GetUserByUsernameAsync(User.getUserName());
        if (user == null)
        {
            return NotFound("Not found any user");
        }
        var result = await photoService.AddPhotoAsync(file);
        if (result.Error != null)
        {
            return BadRequest(result.Error.Message);
        }

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicID = result.PublicId
        };

        if (user.Photos.Count == 0)
        {
            photo.IsMain = true;
        }
        user.Photos.Add(photo);
        if (await _unitOfwork.Complete())
        {
            return CreatedAtAction(nameof(GetUserByName),
                    new { username = user.UserName }, mapper.Map<PhotoDTO>(photo));
        }
        return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfwork.UserRepository.GetUserByUsernameAsync(User.getUserName());
        if (user == null)
        {
            return NotFound();
        }

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
        if (photo == null)
        {
            return NotFound();
        }

        if (photo.IsMain)
        {
            return BadRequest("this is already your main photo");
        }

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMain != null)
        {
            currentMain.IsMain = false;
        }
        photo.IsMain = true;

        if (await _unitOfwork.Complete())
        {
            return NoContent();
        }
        return BadRequest("Problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user  = await _unitOfwork.UserRepository.GetUserByUsernameAsync(User.getUserName());

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null)
        {
            return NotFound();
        }

        if (photo.IsMain)
        {   
            return BadRequest("You can not delete your main photo");
        }

        if (photo.PublicID != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicID);
            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }
        }

        user.Photos.Remove(photo);
        if (await _unitOfwork.Complete())
        {
            return Ok();
        }

        return BadRequest("Problem deleting photo");
    }
}
