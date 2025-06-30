using Domain.Settings;
using Infrastructure.Persistence.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.UnitTests.Tests;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;

    private const string TestIssuer = "TestIssuer";

    private const string TestAudience = "TestAudience";

    private const string TestKey = "A4f9x7J2Ls9P0vXwYrZmQ5Et8BnCvKdC";

    public TokenServiceTests()
    {
        JwtTokenSettings jwtSettings = new JwtTokenSettings
        {
            Key = "A4f9x7J2Ls9P0vXwYrZmQ5Et8BnCvKdC",
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };
        IOptions<JwtTokenSettings> options = Options.Create(jwtSettings);
        _tokenService = new TokenService(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        string userId = "e52c0b07-c8b8-4db9-9d1d-45eea5686291";
        string userName = "inflotestuser";
        List<string> roles = new List<string> { "SuperUser" };
        IEnumerable<Claim> roleClaims = roles.Select((string r) => new Claim("roles", r));
        List<Claim> claims = new List<Claim>
        {
            new Claim("unique_name", userName),
            new Claim("nameid", userId),
            new Claim("sub", userId),
            new Claim("jti", Guid.NewGuid().ToString()),
            new Claim("UserName", userName)
        }.Union(roleClaims).ToList();
        string tokenString = _tokenService.GenerateToken(claims);
        Assert.False(string.IsNullOrWhiteSpace(tokenString));
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken token = handler.ReadJwtToken(tokenString);
        Assert.Equal("TestIssuer", token.Issuer);
        Assert.Equal("TestAudience", token.Audiences.Single());
        Assert.True(token.ValidTo > DateTime.UtcNow);
        Assert.Equal(userName, token.Claims.First((Claim c) => c.Type == "unique_name").Value);
        Assert.Equal(userId, token.Claims.First((Claim c) => c.Type == "nameid").Value);
        Assert.Equal(userId, token.Claims.First((Claim c) => c.Type == "sub").Value);
        List<string> tokenRoles = (from c in token.Claims
                                   where c.Type == "roles"
                                   select c.Value).ToList();
        Assert.Contains(token.Claims, (Predicate<Claim>)((Claim c) => c.Type == "jti"));
        Assert.Equal(userName, token.Claims.First((Claim c) => c.Type == "UserName").Value);
        Assert.Contains("SuperUser", (IEnumerable<string>)tokenRoles);
    }
}
