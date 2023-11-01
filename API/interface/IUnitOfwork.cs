using API;

namespace API;

public interface IUnitOfwork
{
    IUserRepository UserRepository { get; }
    IMessagesRepository MessageRepository { get; }
    ILikeRepository LikesRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}