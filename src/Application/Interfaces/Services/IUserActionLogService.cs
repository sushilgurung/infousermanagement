using Application.Features.User.Queries.GetUser;
using Application.Features.UserActionLog.Queries.UserActionLog;
using Domain.Common;

namespace Application.Interfaces.Services;

public interface IUserActionLogService
{
    Task LogUserActionAsync(UserActionLog userActionLog);
    Task<Result<PaginatedList<GetUserActionLogDto>>> GetLogUserActionAsync(GetUserRequestParameters getUserRequestParameters, Nullable<DateTime> startDate, Nullable<DateTime> endDate, CancellationToken cancellationToken = default);

}
