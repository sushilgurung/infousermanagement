using BlazorClient.Common;
using BlazorClient.Dto;
using BlazorClient.Extensions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorClient.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserService> _logger;
        public UserService(HttpClient httpClient,
        ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// This method retrieves a list of users based on the provided management request.
        /// </summary>
        /// <param name="managementRequest"></param>
        /// <returns></returns>
        public async Task<Result<PaginatedList<UsersDto>>> GetUsersAsync(UserManagementRequestDto managementRequest)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                  nameof(GetUsersAsync), managementRequest);
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Result<PaginatedList<UsersDto>>>(
                    $"api/user/users{managementRequest.GetQueryString()}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {FunctionName}", nameof(GetUsersAsync));
                throw;
                //  return Result<List<UsersDto>>.F("An error occurred while fetching users.");
            }
        }


        /// <summary>
        /// This method retrieves a user by their Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<UsersDto>> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                  nameof(GetUserByIdAsync), id);
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Result<UsersDto>>($"api/user/{id}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {FunctionName}", nameof(GetUserByIdAsync));
                throw;
                //  return Result<List<UsersDto>>.F("An error occurred while fetching users.");
            }
        }

        /// <summary>
        /// This method creates a new user based on the provided user DTO.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<Result<CreateUserDto>> CreateUserAsync(CreateUserDto userDto)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                  nameof(CreateUserAsync), userDto);
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/user", userDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Result<CreateUserDto>>();
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<Result<CreateUserDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return errorResponse;
                }
                return new Result<CreateUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {FunctionName}", nameof(CreateUserAsync));
                throw;
            }
        }

        /// <summary>
        /// This method updates an existing user based on the provided user DTO.
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        public async Task<Result<CreateUserDto>> UpdateUserAsync(int userId, CreateUserDto userDto)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                  nameof(UpdateUserAsync), userDto);
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/user/{userId}", userDto);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Result<CreateUserDto>>();
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<Result<CreateUserDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return errorResponse;
                }
                return new Result<CreateUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {FunctionName}", nameof(UpdateUserAsync));
                throw;
            }
        }

        /// <summary>
        /// This method deletes a user by their Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserAsync(int id)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                  nameof(DeleteUserAsync), id);
            try
            {
                var response = await _httpClient.DeleteAsync($"api/user/{id}");
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Result>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error occurred in {FunctionName}", nameof(DeleteUserAsync));
                return Result.Failure("A network error occurred while attempting to delete the user.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in {FunctionName}", nameof(DeleteUserAsync));
                return Result.Failure("An unexpected error occurred.");
            }
        }
    }
}
