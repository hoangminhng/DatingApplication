using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize]
public class MessageHub : Hub
{
    private readonly IUnitOfwork _unitOfwork;
    private readonly IMapper mapper;
    private readonly IHubContext<PresenceHub> presenceHub;
    public MessageHub(IUnitOfwork unitOfwork,IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        this._unitOfwork = unitOfwork;
        this.mapper = mapper;
        this.presenceHub = presenceHub;
    }

    public async override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];
        var groupName = GetGroupName(Context.User.getUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _unitOfwork.MessageRepository.GetMessageThread(Context.User.getUserName(), otherUser);

        if (_unitOfwork.HasChanges())
        {
            await _unitOfwork.Complete();
        }

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        var username = Context.User.getUserName();

        if (username == createMessageDTO.RecipientUsername.ToLower())
        {
            throw new HubException("You can not send message to yourself");
        }

        var sender = await _unitOfwork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfwork.UserRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

        if (recipient == null)
        {
            throw new HubException("Not found any member to send message");
        }

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUserName = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content,
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var group = await _unitOfwork.MessageRepository.GetMessageGroup(groupName);

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, knownAs = sender.KnownAs});
            }
        }

        _unitOfwork.MessageRepository.AddMessage(message);
        if (await _unitOfwork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
        }
    }

    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _unitOfwork.MessageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.getUserName());

        if (group == null)
        {
            group = new Group(groupName);
            _unitOfwork.MessageRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await _unitOfwork.Complete())
        {
            return group;
        }
        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await _unitOfwork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _unitOfwork.MessageRepository.RemoveConnection(connection);
        if (await _unitOfwork.Complete())
        {
            return group;
        }
        throw new HubException("Failed to remove from group");
    }
}
