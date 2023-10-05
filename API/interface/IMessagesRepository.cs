﻿namespace API;

public interface IMessagesRepository
{
    void AddMessage(Message message);
    void DeleteMesage(Message message);
    Task<Message> GetMessage(int Id);
    Task<PageList<MessageDTO>> GetMessageForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUserName, string recipientUsername);
    Task<bool> SaveAllAsycn();
}
