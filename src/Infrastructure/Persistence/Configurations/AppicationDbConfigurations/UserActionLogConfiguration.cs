namespace Infrastructure.Persistence.Configurations.AppicationDbConfigurations;

public sealed class UserActionLogConfiguration : IEntityTypeConfiguration<UserActionLog>, IApplicationDbConfigurations
{
    public void Configure(EntityTypeBuilder<UserActionLog> builder)
    {
      builder.HasOne(x=>x.ApplicationUser)
             .WithMany(x=>x.UserActionLogs)
             .HasForeignKey(x=>x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
    }
}