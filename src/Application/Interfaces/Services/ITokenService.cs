using System.Security.Claims;

namespace Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(List<Claim> claims);
}