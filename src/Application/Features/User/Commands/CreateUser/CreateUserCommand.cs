using Application.Dto;
using Application.Interfaces.QueueServices;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Domain.Enum;

namespace Application.Features.User.Commands.CreateUser;


public class CreateUserCommand : CreateUserDto, IRequest<IResult>
{

}

public class CreateUserManagementCommandHandler : IRequestHandler<CreateUserCommand, IResult>
{
    private readonly ILogger<CreateUserManagementCommandHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserActionLogQueuePublisher _userActionLogQueuePublisher;
    public CreateUserManagementCommandHandler(
        ILogger<CreateUserManagementCommandHandler> logger,
        IUserRepository userRepository,
        IUserActionLogQueuePublisher userActionLogQueuePublisher
        )
    {
        _logger = logger;
        _userRepository = userRepository;
        _userActionLogQueuePublisher = userActionLogQueuePublisher;
    }

    public async Task<IResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
         nameof(CreateUserManagementCommandHandler), request);
        try
        {
            Domain.Entities.User user = new Domain.Entities.User()
            {
                ForeName = request.ForeName,
                SurName = request.SurName,
                Email = request.Email,
                DateOfBirth = request.DateOfBirth,
                IsActive = request.IsActive
            };
            _logger.LogInformation("Creating user with data: {@UserData}", user);
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
            var result = Result.Success("User created successfully.");

            await _userActionLogQueuePublisher.PublishAsync(
                action: UserActionTypeEnum.Created,
                resourceType: ResourceTypeEnum.User,
                description: $"User created with ID: {user.Id}, Email: {user.Email}, Name: {user.ForeName} {user.SurName}"
            ).ConfigureAwait(false);

            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(CreateUserManagementCommandHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}
