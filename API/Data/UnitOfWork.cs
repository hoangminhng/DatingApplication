using AutoMapper;

namespace API.Data;

public class UnitOfWork : IUnitOfwork
{
    private readonly DataContext dataContext;
    private readonly IMapper mapper;
    public UnitOfWork(DataContext dataContext, IMapper mapper)
    {
        this.dataContext = dataContext;
        this.mapper = mapper;
    }
    public IUserRepository UserRepository => new UserRepository(dataContext, mapper);
    public IMessagesRepository MessageRepository => new MessagesRepository(dataContext, mapper);
    public ILikeRepository LikesRepository => new LikesRepository(dataContext);
    public async Task<bool> Complete()
    {
        return await dataContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return dataContext.ChangeTracker.HasChanges();
    }
}