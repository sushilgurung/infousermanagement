using BlazorClient.Common;
using BlazorClient.Dto;

namespace BlazorClient.Services.UserService
{
    public interface IUserService
    {
        Task<Result<PaginatedList<UsersDto>>> GetUsersAsync(UserManagementRequestDto managementRequest);
        Task<Result<CreateUserDto>> CreateUserAsync(CreateUserDto userDto);
        Task<Result<CreateUserDto>> UpdateUserAsync(int userId, CreateUserDto userDto);
        Task<Result<UsersDto>> GetUserByIdAsync(int id);
        Task<Result> DeleteUserAsync(int id);
    }
}
