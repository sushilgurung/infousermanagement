namespace Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    private readonly ICurrentUserService _currentUserService;
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // Used 'new' keyword to explicitly hide the inherited member
    public DbSet<User> UsersManagement { get; set; }
    public DbSet<UserActionLog> UserActionLogs { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedBy = _currentUserService.UserId;
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var configType = typeof(IApplicationDbConfigurations);
        var assembly = configType.Assembly;
        string namespaceName = configType.Namespace;

        var configurations = assembly.GetTypes()
            .Where(t => t.Namespace == namespaceName
            && t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
            && !t.IsInterface
            && !t.IsAbstract);

        foreach (var config in configurations)
        {
            var configurationInstance = Activator.CreateInstance(config);
            builder.ApplyConfiguration((dynamic)configurationInstance);
        }
        base.OnModelCreating(builder);
    }
}
