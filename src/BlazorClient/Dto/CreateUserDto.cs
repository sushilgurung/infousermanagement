using System.ComponentModel.DataAnnotations;

namespace BlazorClient.Dto
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "First name is required")]
        public string ForeName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string SurName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Date of birth is required")]
        public DateOnly DateOfBirth { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public bool IsActive { get; set; }
    }
}
