using System.Text.Json;
using Application.Features.User.Command.UpdateUser;
using Application.Interfaces.QueueServices;
using Application.Interfaces.Repositories;
using Domain.Common;
using Domain.Enum;

namespace Application.Features.User.Command.DeleteUser;


public class DeleteUserCommand : IRequest<IResult>
{
    public int UserId { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, IResult>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserActionLogQueuePublisher _userActionLogQueuePublisher;
    public DeleteUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IUserRepository userRepository,
        IUserActionLogQueuePublisher userActionLogQueuePublisher
        )
    {
        _logger = logger;
        _userRepository = userRepository;
        _userActionLogQueuePublisher = userActionLogQueuePublisher;
    }

    public async Task<IResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
         nameof(DeleteUserCommandHandler), request);
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId).ConfigureAwait(false);
            if (user is null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return Results.Json(Result.Failure($"User with ID {request.UserId} not found."), statusCode: StatusCodes.Status404NotFound);
            }
            await _userRepository.RemoveAsync(user);
            await _userRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await _userActionLogQueuePublisher.PublishAsync(
            action: UserActionTypeEnum.Viewed,
            resourceType: ResourceTypeEnum.User,
              $"User deleted with ID: {user.Id}, Users deleted: {JsonSerializer.Serialize(user)}"
            ).ConfigureAwait(false);

            return Results.Ok(Result.Success("User has been deleted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(DeleteUserCommandHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}
