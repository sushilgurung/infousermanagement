using Domain.Enum;

namespace Domain.Entities;

[Table("UserActionLogs")]
public class UserActionLog
{
    [Key]
    public virtual int Id { get; set; }
    public string UserId { get; set; }
    public UserActionTypeEnum Action { get; set; }
    public ResourceTypeEnum ResourceType { get; set; }
    public DateTime PerformedOn { get; set; } = DateTime.UtcNow;
    public string Description { get; set; }
    public string IpAddress { get; set; }

    [ForeignKey("UserId")]
    public virtual ApplicationUser ApplicationUser { get; set; }
}
