
using Application.Features.User.Queries.GetUser;
using Application.Features.UserActionLog.Queries.UserActionLog;
using Application.Interfaces.Repositories;
using Domain.Common;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;

namespace Application.Features.UserActionLog.Query.GetUserActionLogById;

public class GetUserActionLogByIdQuery : IRequest<IResult>
{
    public int LogId { get; set; }
}

public class GetUserActionLogByIdQueryHandler : IRequestHandler<GetUserActionLogByIdQuery, IResult>
{
    private readonly IUserActionLogRepository _userActionLogRepository;
    private UserManager<ApplicationUser> _userManager;
    private readonly ILogger<GetUserActionLogByIdQueryHandler> _logger;
    public GetUserActionLogByIdQueryHandler(
        IUserActionLogRepository userActionLogRepository,
                UserManager<ApplicationUser> userManager,
        ILogger<GetUserActionLogByIdQueryHandler> logger
        )
    {
        _userActionLogRepository = userActionLogRepository;
        _userManager = userManager;
        this._logger = logger;
    }
    public async Task<IResult> Handle(GetUserActionLogByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
            nameof(GetUserActionLogByIdQueryHandler), request);
        try
        {

            var logEntry = await (from log in _userActionLogRepository.Queryable
                                  join user in _userManager.Users
                                      on log.UserId equals user.Id into userJoin
                                  from user in userJoin.DefaultIfEmpty()
                                  where log.Id == request.LogId
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
                                  }).FirstOrDefaultAsync(cancellationToken);
            if (logEntry is null)
            {
                _logger.LogWarning("User action log with Id {Id} not found", request.LogId);
                return Results.Json(Result.Failure($"User action log with Id {request.LogId} not found."), statusCode: StatusCodes.Status404NotFound);
            }
            return Results.Ok(Result<GetUserActionLogDto>.Success(logEntry, "User action log retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserActionLogByIdQueryHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}


