namespace API;

public interface IMessagesRepository
{
    void AddMessage(Message message);
    void DeleteMesage(Message message);
    Task<Message> GetMessage(int id);
    Task<PageList<MessageDTO>> GetMessageForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);
}
