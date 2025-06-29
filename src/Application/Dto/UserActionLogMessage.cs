

using Domain.Enum;

namespace Application.Dto;
public record UserActionLogMessage(
    string UserId,
    UserActionTypeEnum Action,
    ResourceTypeEnum ResourceType,
    DateTime PerformedOn,
    string Description,
    string IpAddress);
