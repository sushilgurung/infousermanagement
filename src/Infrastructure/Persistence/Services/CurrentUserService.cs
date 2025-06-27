
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Persistence.Services;

    public class CurrentUserService : ICurrentUserService
    {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        Roles = httpContextAccessor.HttpContext?.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        UserPublicIpAddress = httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }

    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string UserPublicIpAddress { get; }

    public List<string> Roles { get; }
}


