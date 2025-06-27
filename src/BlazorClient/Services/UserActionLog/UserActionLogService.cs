using BlazorClient.Common;
using BlazorClient.Dto;
using BlazorClient.Extensions;
using System.Net.Http.Json;

namespace BlazorClient.Services.UserActionLog
{
    public class UserActionLogService : IUserActionLogService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserActionLogService> _logger;

        public UserActionLogService(HttpClient httpClient, ILogger<UserActionLogService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of user action logs based on the query parameters.
        /// </summary>
        /// <param name="query">The query parameters for filtering and pagination.</param>
        /// <returns>A Result wrapping a PaginatedList of UserActionLogDto.</returns>
        public async Task<Result<PaginatedList<UserActionLogDto>>> GetUserActionLogsAsync(GetUserActionQuery query)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                   nameof(GetUserActionLogsAsync), query);
            try
            {
                var queryString = query.GetQueryString();

                var response = await _httpClient.GetFromJsonAsync<Result<PaginatedList<UserActionLogDto>>>(
                    $"api/userActionLog/useractionloggers{queryString}");

                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error occurred in {FunctionName}", nameof(GetUserActionLogsAsync));
                return Result.Failure<PaginatedList<UserActionLogDto>>("A network error occurred while attempting to fetch user action logs.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in {FunctionName}", nameof(GetUserActionLogsAsync));
                return Result.Failure<PaginatedList<UserActionLogDto>>("An unexpected error occurred.");
            }
        }

        /// <summary>
        /// This method retrieves a specific user action log by its Id.
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public async Task<Result<UserActionLogDto>> GetUserActionLogByIdAsync(int logId)
        {
            _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                   nameof(GetUserActionLogByIdAsync), logId);
            try
            {
                var response = await _httpClient.GetFromJsonAsync<Result<UserActionLogDto>>($"api/userActionLog/useractionloggers/{logId}");
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error occurred in {FunctionName}", nameof(GetUserActionLogByIdAsync));
                return Result.Failure<UserActionLogDto>("A network error occurred while attempting to fetch the user action log.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in {FunctionName}", nameof(GetUserActionLogByIdAsync));
                return Result.Failure<UserActionLogDto>("An unexpected error occurred.");
            }
        }

    }
}
