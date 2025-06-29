using BlazorClient.Common;
using BlazorClient.Dto;

namespace BlazorClient.Services.AuthService
{
    public interface IAuthService
    {
        Task<Result<UserDetailsDto>> LoginAsync(string username, string password);
        Task LogoutAsync();
        Task NotifyUserAuthenticationAsync();
        Task<Result<TokenResponseDto>> TryRefreshTokenAsync();
    }
}
