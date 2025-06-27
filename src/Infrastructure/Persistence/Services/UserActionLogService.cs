
using Application.Features.User.Command.UpdateUser;
using Application.Features.User.Queries.GetUser;
using Application.Features.UserActionLog.Queries.UserActionLog;
using Azure.Core;
using Domain.Common;
using Domain.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Services;

public class UserActionLogService : IUserActionLogService
{
    private readonly ILogger<UserActionLogService> _logger;
    private readonly IUserActionLogRepository _userActionLogRepository;
    private readonly ICurrentUserService _currentUserService;

    private readonly ApplicationDbContext _dbContext;
    public UserActionLogService(
        IUserActionLogRepository userActionLogRepository,
        ILogger<UserActionLogService> logger,
        ICurrentUserService currentUserService,
        ApplicationDbContext dbContext
        )
    {
        _userActionLogRepository = userActionLogRepository;
        _logger = logger;
        _currentUserService = currentUserService;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <summary>
    /// This method logs user actions by saving the UserActionLog object to the database.
    /// </summary>
    /// <param name="userActionLog"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task LogUserActionAsync(UserActionTypeEnum action, ResourceTypeEnum resourceType, string description)
    {
        try
        {
            UserActionLog userActionLog = new UserActionLog
            {
                UserId = _currentUserService.UserId,
                Action = action,
                ResourceType = resourceType,
                Description = description,
                PerformedOn = DateTime.UtcNow,
                IpAddress = _currentUserService.UserPublicIpAddress
            };

            await _userActionLogRepository.AddAsync(userActionLog);
            await _userActionLogRepository.SaveChangesAsync();
            _logger.LogInformation("User action log saved successfully for user {UserId} with action {Action} at {PerformedOn}.",
                userActionLog.UserId, userActionLog.Action, userActionLog.PerformedOn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging user action for user {UserId} with action {Action}.", _currentUserService.UserId, action);
        }
    }


    /// <summary>
    /// This method retrieves user action logs based on the provided parameters.
    /// </summary>
    /// <param name="getUserRequestParameters"></param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<PaginatedList<GetUserActionLogDto>>> GetLogUserActionAsync(GetUserRequestParameters getUserRequestParameters, Nullable<DateTime> startDate, Nullable<DateTime> endDate, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
            nameof(GetLogUserActionAsync), getUserRequestParameters);
        try
        {
            var query = (from log in _userActionLogRepository.Queryable
                         join user in _dbContext.Users
                             on log.UserId equals user.Id into userJoin
                         from user in userJoin.DefaultIfEmpty()
                         where (startDate == null || log.PerformedOn >= startDate) &&
                               (endDate == null || log.PerformedOn <= endDate)
                         orderby log.PerformedOn descending
                         select new GetUserActionLogDto
                         {
                             Id = log.Id,
                             UserId = log.UserId,
                             UserName = user != null ? user.UserName : "Unknown",
                             Action = log.Action.ToString(),
                             ResourceType = log.ResourceType.ToString(),
                             Description = log.Description,
                             IpAddress = log.IpAddress,
                             PerformedOn = log.PerformedOn
                         });


            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((getUserRequestParameters.PageNumber - 1) * getUserRequestParameters.PageSize)
                .Take(getUserRequestParameters.PageSize)
                .ToListAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} user action logs for page {PageNumber} with page size {PageSize}.",
                items.Count, getUserRequestParameters.PageNumber, getUserRequestParameters.PageSize);

            PaginatedList<GetUserActionLogDto> pagination = PaginatedList<GetUserActionLogDto>.Create(items, getUserRequestParameters.PageSize, getUserRequestParameters.PageNumber, totalCount);
            var result = Result.Success(pagination, "user retrive successfully.");
            return Result<PaginatedList<GetUserActionLogDto>>.Success(pagination, "User action logs retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving user action logs for request: {@RequestData}", getUserRequestParameters);
            throw;
        }

    }
}



