namespace Infrastructure.Persistence.Configurations.AppicationDbConfigurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>, IApplicationDbConfigurations
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasOne(x => x.User)
               .WithMany(x => x.RefreshTokens)
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
