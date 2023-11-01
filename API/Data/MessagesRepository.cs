using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessagesRepository : IMessagesRepository
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    public MessagesRepository(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }

    public void AddGroup(Group group)
    {
        dataContext.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        dataContext.Messages.Add(message);
    }

    public void DeleteMesage(Message message)
    {
        dataContext.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await dataContext.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await dataContext.Messages.FindAsync(id);
    }

    public Task<PageList<MessageDTO>> GetMessageForUser(MessageParams messageParams)
    {
        var query = dataContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.Recipient.UserName == messageParams.Username
                && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
                && u.SenderDeleted == false),
            _ => query.Where(u => u.Recipient.UserName == messageParams.Username
                && u.RecipientDeleted == false && u.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

        return PageList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);

    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername)
    {
        var messages = dataContext.Messages
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                     m.SenderUserName == recipientUsername ||
                     m.RecipientUsername == recipientUsername && m.SenderDeleted == false &&
                     m.SenderUserName == currentUserName
            )
            .OrderBy(m => m.MessageSent)
            .AsQueryable();

        var unReadMessages = messages.Where(m => m.DateRead == null
                                                 && m.RecipientUsername == currentUserName).ToList();

        if (unReadMessages.Any())
        {
            foreach (var message in unReadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return await messages.ProjectTo<MessageDTO>(mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        dataContext.Connections.Remove(connection);
    }
}
