using Application.Features.User.Queries.GetUser;
using Application.Features.UserActionLog.Queries.UserActionLog;
using Domain.Common;
using Domain.Enum;

namespace Application.Interfaces.Services;

public interface IUserActionLogService
{
    Task LogUserActionAsync(UserActionTypeEnum action, ResourceTypeEnum resourceType, string description);
    Task<Result<PaginatedList<GetUserActionLogDto>>> GetLogUserActionAsync(GetUserRequestParameters getUserRequestParameters, Nullable<DateTime> startDate, Nullable<DateTime> endDate, CancellationToken cancellationToken = default);

}