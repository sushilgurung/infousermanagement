namespace Domain.Entities;

[Table("Users")]
public class User : BaseEntity
{
    [Key]
    public virtual int Id { get; set; }
    public string ForeName { get; set; }
    public string SurName { get; set; }
    public string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}