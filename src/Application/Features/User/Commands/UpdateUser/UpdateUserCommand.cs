using System.Text;
using Application.Features.User.Commands.CreateUser;
using Application.Interfaces.QueueServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Enum;

namespace Application.Features.User.Command.UpdateUser;


public class UpdateUserCommand : IRequest<IResult>
{
    public int UserId { get; set; }
    public string ForeName { get; set; }
    public string SurName { get; set; }
    public string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
{
    private readonly ILogger<UpdateUserCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserActionLogService _userActionLogService;
    private readonly IUserActionLogQueuePublisher _userActionLogQueuePublisher;
    public UpdateUserCommandHandler(
        ILogger<UpdateUserCommandHandler> logger,
        IUserRepository userRepository,
        IUserActionLogService userActionLogService,
         IUserActionLogQueuePublisher userActionLogQueuePublisher
        )
    {
        _logger = logger;
        _userRepository = userRepository;
        _userActionLogService = userActionLogService;
        _userActionLogQueuePublisher = userActionLogQueuePublisher;
    }

    public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
         nameof(UpdateUserCommandHandler), request);
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId).ConfigureAwait(false);
            if (user is null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", request.UserId);
                return Results.Json(Result.Failure($"User with ID {request.UserId} not found."), statusCode: StatusCodes.Status404NotFound);
            }
            _logger.LogInformation("User found with data: {@UserData}", user);

            user.ForeName = request.ForeName;
            user.SurName = request.SurName;
            user.Email = request.Email;
            user.DateOfBirth = request.DateOfBirth;
            user.IsActive = request.IsActive;

            var oldForeName = user.ForeName;
            var oldSurName = user.SurName;
            var oldEmail = user.Email;
            var oldDateOfBirth = user.DateOfBirth;
            var oldIsActive = user.IsActive;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            var changesDescription = new StringBuilder();
            if (oldForeName != user.ForeName)
                changesDescription.AppendLine($"ForeName: '{oldForeName}' => '{user.ForeName}'");
            if (oldSurName != user.SurName)
                changesDescription.AppendLine($"SurName: '{oldSurName}' => '{user.SurName}'");
            if (oldEmail != user.Email)
                changesDescription.AppendLine($"Email: '{oldEmail}' => '{user.Email}'");
            if (oldDateOfBirth != user.DateOfBirth)
                changesDescription.AppendLine($"DateOfBirth: '{oldDateOfBirth:yyyy-MM-dd}' => '{user.DateOfBirth:yyyy-MM-dd}'");
            if (oldIsActive != user.IsActive)
                changesDescription.AppendLine($"IsActive: '{oldIsActive}' => '{user.IsActive}'");

            // await _userActionLogService.LogUserActionAsync(
            //    UserActionTypeEnum.Updated,
            //    ResourceTypeEnum.User,
            //    $"User updated with ID: {user.Id}, Email: {user.Email}"
            //).ConfigureAwait(false);

            await _userActionLogQueuePublisher.PublishAsync(
             UserActionTypeEnum.Updated,
               ResourceTypeEnum.User,
               $"User updated with ID: {user.Id}, Email: {user.Email}"
       ).ConfigureAwait(false);

            return Results.Ok(Result.Success("User has been update successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(UpdateUserCommandHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}
