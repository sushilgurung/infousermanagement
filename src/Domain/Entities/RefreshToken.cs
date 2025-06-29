namespace Domain.Entities;

[Table("RefreshTokens")]
public class RefreshToken
{
    [Key]
    public virtual int Id { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual ApplicationUser User { get; set; }
}
