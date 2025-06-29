using Application.Dto;
using Domain.Enum;

namespace Application.Interfaces.QueueServices;
public interface IUserActionLogQueuePublisher
{
    Task PublishAsync(UserActionTypeEnum action, ResourceTypeEnum resourceType, string description);
  
}
