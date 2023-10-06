using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;

namespace API;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessagesRepository messagesRepository;
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IHubContext<PresenceHub> presenceHub;
    public MessageHub(IMessagesRepository messagesRepository, IUserRepository userRepository
        , IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        this.messagesRepository = messagesRepository;
        this.userRepository = userRepository;
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

        var messages = await messagesRepository.GetMessageThread(Context.User.getUserName(), otherUser);

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

        var sender = await userRepository.GetUserByUsernameAsync(username);
        var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

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
        var group = await messagesRepository.GetMessageGroup(groupName);

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

        messagesRepository.AddMessage(message);
        if (await messagesRepository.SaveAllAsycn())
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
        var group = await messagesRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.getUserName());

        if (group == null)
        {
            group = new Group(groupName);
            messagesRepository.AddGroup(group);
        }

        group.Connections.Add(connection);

        if (await messagesRepository.SaveAllAsycn())
        {
            return group;
        }
        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        var group = await messagesRepository.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        messagesRepository.RemoveConnection(connection);
        if (await messagesRepository.SaveAllAsycn())
        {
            return group;
        }
        throw new HubException("Failed to remove from group");
    }
}
