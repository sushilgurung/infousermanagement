using Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement.UnitTests.Tests;

public class IdentityOptionsAccessorServiceTests
{
    private readonly IdentityOptionsAccessorService _service;

    private readonly string _allowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    public IdentityOptionsAccessorServiceTests()
    {
        Mock<ILogger<IdentityOptionsAccessorService>> loggerMock = new Mock<ILogger<IdentityOptionsAccessorService>>();
        IdentityOptions identityOptions = new IdentityOptions
        {
            User = new UserOptions
            {
                AllowedUserNameCharacters = _allowedCharacters
            }
        };
        IOptions<IdentityOptions> optionsMock = Options.Create(identityOptions);
        _service = new IdentityOptionsAccessorService(loggerMock.Object, optionsMock);
    }

    [Theory]
    [InlineData(new object[] { "validUser123", true })]
    [InlineData(new object[] { "Invalid User!", false })]
    [InlineData(new object[] { "user.name@example.com", true })]
    [InlineData(new object[] { "user-name", true })]
    [InlineData(new object[] { "user+name", true })]
    public async Task IsUserNameValid_ShouldReturnExpectedResult(string userName, bool expected)
    {
        Assert.Equal(expected, await _service.IsUserNameValid(userName));
    }

    [Fact]
    public void GetAllowedUserNameCharacters_ShouldReturnConfiguredCharacters()
    {
        string result = _service.GetAllowedUserNameCharacters();
        Assert.Equal(_allowedCharacters, result);
    }
}
