using Application.Interfaces.QueueServices;
using Domain.Enum;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.QueueServices;
public class UserActionLogQueuePublisher : IUserActionLogQueuePublisher
{
    private readonly IQueueService _queueService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<UserActionLogService> _logger;
    private readonly IConfiguration _configuration;
    public UserActionLogQueuePublisher(
        IConfiguration configuration,
        IQueueService queueService,
        ICurrentUserService currentUserService,
        ILogger<UserActionLogService> logger)
    {
        _configuration = configuration;
      
        _queueService = queueService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    /// <summary>
    /// Publishes a user action log message to the service bus.
    /// </summary>
    /// <remarks>This method serializes the provided <paramref name="message"/> into JSON format and sends it
    /// as a service bus message using the configured sender. Ensure that the sender is properly initialized before
    /// calling this method.</remarks>
    /// <param name="message">The user action log message to be published. This parameter cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task PublishAsync(UserActionTypeEnum action, ResourceTypeEnum resourceType, string description)
    {
        try
        {
            var subscriptionQueueName = _configuration.GetValue<string>("TopicAndQueueNames:UseractionlogsQueue");
            UserActionLog message = new UserActionLog
            {
                UserId = _currentUserService.UserId,
                Action = action,
                ResourceType = resourceType,
                Description = description,
                PerformedOn = DateTime.UtcNow,
                IpAddress = _currentUserService.UserPublicIpAddress
            };
            await _queueService.SendMessageAsync(message, subscriptionQueueName).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while logging user action for user {UserId} with action {Action}.", _currentUserService.UserId, action);
        }
    }

}
