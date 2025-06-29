namespace BlazorClient.Dto
{
    public class UserDetailsDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public string[] Roles { get; set; }
        public string RefreshToken { get; set; }
    }
}
