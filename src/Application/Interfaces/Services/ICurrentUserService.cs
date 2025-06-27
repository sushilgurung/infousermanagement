namespace Application.Interfaces.Services;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserPublicIpAddress { get; }
    List<string> Roles { get; }
}