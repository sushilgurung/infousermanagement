namespace Infrastructure.Persistence.Repositories;
public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}