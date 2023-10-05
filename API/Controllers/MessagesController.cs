using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API;

public class MessagesController : BaseApiControllers
{
    private readonly IUserRepository userRepository;
    private readonly IMessagesRepository messagesRepository;
    private readonly IMapper mapper;
    public MessagesController(IUserRepository userRepository, IMessagesRepository messagesRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.messagesRepository = messagesRepository;
        this.mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var username = User.getUserName();
        if (username == createMessageDTO.RecipientUsername.ToLower())
        {
            return BadRequest("You can not send message to yourself");
        }

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient == null)
        {
            return NotFound();
        }
        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        messagesRepository.AddMessage(message);

        if (await messagesRepository.SaveAllAsycn())
        {
            return Ok(mapper.Map<MessageDTO>(message));
        }
        return BadRequest("Fail to send message");
    }

    [HttpGet]
    public async Task<ActionResult<PageList<MessageDTO>>> GetMessageForUser([FromQuery] MessageParams messageParams)
    {
        messageParams.Username = User.getUserName();

        var messages = await messagesRepository.GetMessageForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
                                                        messages.TotalCount, messages.TotalPages));
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
    {
        var currentUserName = User.getUserName();
        return Ok(await messagesRepository.GetMessageThread(currentUserName, username));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int Id)
    {
        var username = User.getUserName();

        var message = await messagesRepository.GetMessage(Id);

        if (message.SenderUserName != username && message.RecipientUsername != username)
        {
            return BadRequest("wrong");
        }

        if (message.SenderUserName == username)     message.SenderDeleted = true;
        
        if (message.RecipientUsername == username)  message.RecipientDeleted = true;
        
        if (message.SenderDeleted && message.RecipientDeleted)  messagesRepository.DeleteMesage(message);
        
        if(await messagesRepository.SaveAllAsycn()) return Ok();

        return BadRequest("Problem in delete message");
    }
}
