
namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<UserActionLog> UserActionLogs { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
