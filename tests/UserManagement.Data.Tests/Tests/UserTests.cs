using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UserManagement.Data.Tests.Fixture;
using Xunit;

public class UserTests : IClassFixture<ApplicationDbContextTestFixture>
{
    private readonly ApplicationDbContextTestFixture _fixture;

    public UserTests(ApplicationDbContextTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ShouldInsertUserInTestContainerDb()
    {
        //Arrange
        var user = new User
        {
            ForeName = "Test",
            SurName = "Case",
            Email = "test.case@example.com"
        };
        //Act
        using var dbContext = new ApplicationDbContext(_fixture.DbContextOptions, _fixture.CurrentUserServiceMock.Object);
        dbContext.UsersManagement.Add(user);
        await dbContext.SaveChangesAsync();

        var exists = await dbContext.UsersManagement.AnyAsync(u => u.Email == user.Email);
        //Assert
        Assert.True(exists);
    }
}
