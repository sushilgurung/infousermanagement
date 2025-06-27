using Domain.Entities;
using Domain.Enum;
using FluentAssertions;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UserManagement.Data.Tests.Fixture;
using Xunit;

public class UserActionLogTests : IClassFixture<ApplicationDbContextTestFixture>
{
    private readonly ApplicationDbContext _dbContext;

    public UserActionLogTests(ApplicationDbContextTestFixture fixture)
    {
        _dbContext = new ApplicationDbContext(fixture.DbContextOptions, fixture.CurrentUserServiceMock.Object);
    }

    [Fact]
    public async Task ShouldInsertAndRetrieveUserActionLog()
    {
        // Arrange: Create a user first
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "testuser@example.com"
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        // Act: Insert action log
        var log = new UserActionLog
        {
            UserId = user.Id,
            Action = UserActionTypeEnum.Created,
            ResourceType = ResourceTypeEnum.User,
            PerformedOn = DateTime.UtcNow,
            Description = "User logged in",
            IpAddress = "192.168.1.10"
        };

        await _dbContext.UserActionLogs.AddAsync(log);
        await _dbContext.SaveChangesAsync();

        // Assert
        var savedLog = await _dbContext.UserActionLogs
            .Include(x => x.ApplicationUser)
            .FirstOrDefaultAsync(x => x.Id == log.Id);

        savedLog.Should().NotBeNull();
        savedLog.UserId.Should().Be(user.Id);
        savedLog.Action.Should().Be(UserActionTypeEnum.Created);
        savedLog.Description.Should().Be("User logged in");
        savedLog.ApplicationUser.UserName.Should().Be("testuser");
    }
}
