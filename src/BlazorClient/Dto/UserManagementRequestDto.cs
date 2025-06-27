namespace BlazorClient.Dto
{
    public class UserManagementRequestDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool? IsActive { get; set; }
    }
}
