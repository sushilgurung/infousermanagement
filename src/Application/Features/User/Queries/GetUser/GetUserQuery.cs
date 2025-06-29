using System.Text.Json;
using Application.Interfaces.QueueServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.User.Queries.GetUser;

public class GetUserQuery : GetUserRequestParameters, IRequest<IResult>;


public class GetUserManagementQueryHandler : IRequestHandler<GetUserQuery, IResult>
{
    private readonly ILogger<GetUserManagementQueryHandler> _logger;
    private readonly IUserRepository _userManagementRepository;
    private readonly IUserActionLogQueuePublisher _userActionLogQueuePublisher;
    public GetUserManagementQueryHandler(
        ILogger<GetUserManagementQueryHandler> logger,
        IUserRepository userManagementRepository,
        IUserActionLogQueuePublisher userActionLogQueuePublisher)
    {
        _logger = logger;
        _userManagementRepository = userManagementRepository;
        _userActionLogQueuePublisher = userActionLogQueuePublisher;
    }
    public async Task<IResult> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
            nameof(GetUserManagementQueryHandler), request);
        try
        {
            var query = _userManagementRepository.Queryable.Where(x => request.IsActive == null || x.IsActive == request.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new GetUserDto
            {
                Id = x.Id,
                ForeName = x.ForeName,
                SurName = x.SurName,
                Email = x.Email,
                DateOfBirth = x.DateOfBirth,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

            PaginatedList<GetUserDto> pagination = PaginatedList<GetUserDto>.Create(items, request.PageSize, request.PageNumber, totalCount);
            var result = Result.Success(pagination, "user retrive successfully.");

            await _userActionLogQueuePublisher.PublishAsync(
              action: UserActionTypeEnum.Viewed,
              resourceType: ResourceTypeEnum.User,
             $"Retrieved user list, page {request.PageNumber}, page size {request.PageSize}").ConfigureAwait(false);
             


            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserManagementQueryHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}


