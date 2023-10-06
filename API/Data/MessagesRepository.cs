
using API.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

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

    public async Task<Connection> GetConnection(string ConnectionId)
    {
        return await dataContext.Connections.FindAsync(ConnectionId);
    }

    public async Task<Group> GetGroupForConnection(string ConnectionId)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == ConnectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int Id)
    {
        return await dataContext.Messages.FindAsync(Id);
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

    public async Task<Group> GetMessageGroup(string GroupName)
    {
        return await dataContext.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == GroupName);

    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername)
    {
        var messages = await dataContext.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false &&
                m.SenderUserName == recipientUsername ||
                m.RecipientUsername == recipientUsername && m.SenderDeleted == false &&
                m.SenderUserName == currentUserName
            )
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unReadMessages = messages.Where(m => m.DateRead == null
        && m.RecipientUsername == currentUserName).ToList();

        if (unReadMessages.Any())
        {
            foreach (var message in unReadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await dataContext.SaveChangesAsync();
        }
        return mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public void RemoveConnection(Connection connection)
    {
        dataContext.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsycn()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }
}
