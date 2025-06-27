
using Application.Features.User.Queries.GetUser;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Features.User.Query.GetUserById;

public class GetUserByIdQuery : IRequest<IResult>
{
    public int UserId { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, IResult>
{
    private readonly ILogger<GetUserManagementQueryHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserActionLogService _userActionLogService;
    public GetUserByIdQueryHandler(
        ILogger<GetUserManagementQueryHandler> logger,
        IUserRepository userRepository,
        IUserActionLogService userActionLogService
        )
    {
        _logger = logger;
        _userRepository = userRepository;
        _userActionLogService = userActionLogService;
    }
    public async Task<IResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
            nameof(GetUserByIdQueryHandler), request);
        try
        {
            var user = await _userRepository.Queryable.AsNoTracking().Select(s => new GetUserDto
            {
                Id = s.Id,
                ForeName = s.ForeName,
                SurName = s.SurName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                IsActive = s.IsActive
            }).FirstOrDefaultAsync(x => x.Id == request.UserId).ConfigureAwait(false);
            if (user is null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return Results.Json(Result.Failure($"User with ID {request.UserId} not found."), statusCode: StatusCodes.Status404NotFound);
            }

            await _userActionLogService.LogUserActionAsync(
              action: UserActionTypeEnum.Viewed,
              resourceType: ResourceTypeEnum.User,
              description: $"Viewed user: {JsonSerializer.Serialize(user)}"
          ).ConfigureAwait(false);

            return Results.Ok(Result<GetUserDto>.Success(user, "User found successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserByIdQueryHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}


