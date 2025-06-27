
using BlazorClient.Common;
using BlazorClient.Dto;

namespace BlazorClient.Services.UserActionLog
{
    public interface IUserActionLogService
    {
        Task<Result<PaginatedList<UserActionLogDto>>> GetUserActionLogsAsync(GetUserActionQuery query);
        Task<Result<UserActionLogDto>> GetUserActionLogByIdAsync(int logId);
    }
}
