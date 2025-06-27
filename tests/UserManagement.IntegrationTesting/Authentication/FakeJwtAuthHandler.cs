using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UserManagement.IntegrationTesting.Authentication;

public class FakeJwtAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public FakeJwtAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
         {
            new Claim(ClaimTypes.Name, "SuperUser"),
            new Claim(ClaimTypes.NameIdentifier, "e52c0b07-c8b8-4db9-9d1d-45eea5686291"),
            new Claim("jti", "8bd2a22d-d198-49a8-a4fc-c8d6a7addbdc"),
            new Claim("roles", "SuperUser"),
            new Claim(ClaimTypes.Role, "SuperUser"), 
            new Claim("iss", "Inflo"),
            new Claim("aud", "Inflo")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}