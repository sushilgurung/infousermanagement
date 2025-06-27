
using Application.Features.User.Queries.GetUser;
using Application.Interfaces.Services;

namespace Application.Features.UserActionLog.Query.GetUserActionLog;

public class GetUserActionLogQuery : GetUserRequestParameters, IRequest<IResult>
{
    public Nullable<DateTime> StartDate { get; set; }
    public Nullable<DateTime> EndDate { get; set; }
}

public class GetUserActionLogQueryHandler : IRequestHandler<GetUserActionLogQuery, IResult>
{
    private readonly ILogger<GetUserActionLogQueryHandler> _logger;
    private readonly IUserActionLogService _userActionLogService;
    public GetUserActionLogQueryHandler(
        ILogger<GetUserActionLogQueryHandler> logger,
        IUserActionLogService userActionLogService
        )
    {
        this._logger = logger;
        this._userActionLogService = userActionLogService;
    }
    public async Task<IResult> Handle(GetUserActionLogQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
            nameof(GetUserActionLogQueryHandler), request);
        try
        {
            var result = await _userActionLogService.GetLogUserActionAsync(request, request.StartDate, request.EndDate, cancellationToken);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserActionLogQueryHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}


