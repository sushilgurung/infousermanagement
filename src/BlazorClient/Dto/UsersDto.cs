namespace BlazorClient.Dto
{
    public class UsersDto
    {
        public int Id { get; set; }
        public string ForeName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }
}
