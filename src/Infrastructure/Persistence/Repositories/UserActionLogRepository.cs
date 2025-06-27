namespace Infrastructure.Persistence.Repositories;
public class UserActionLogRepository : Repository<UserActionLog>, IUserActionLogRepository
{
    public UserActionLogRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}