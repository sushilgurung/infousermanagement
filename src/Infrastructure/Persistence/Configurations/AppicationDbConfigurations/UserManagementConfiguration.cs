namespace Infrastructure.Persistence.Configurations.AppicationDbConfigurations;

public sealed class UserManagementConfiguration : IEntityTypeConfiguration<User>, IApplicationDbConfigurations
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
      
    }
}