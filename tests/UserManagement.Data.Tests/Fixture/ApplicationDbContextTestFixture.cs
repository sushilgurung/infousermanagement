using Application.Interfaces.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Testcontainers.MsSql;

namespace UserManagement.Data.Tests.Fixture;
public class ApplicationDbContextTestFixture : IAsyncLifetime
{
    public IContainer _mssqlContainer;
    public DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }
    public Mock<ICurrentUserService> CurrentUserServiceMock { get; private set; }

    private const int MsSqlPort = 1433;
    private const string Username = "sa";
    private const string Password = "P@ssw0rd!2025#Secure";
    private const string DatabaseName = "TestDb";

    internal string HostName => _mssqlContainer.Hostname;
    internal int Port => _mssqlContainer.GetMappedPublicPort(MsSqlPort);
    internal string Sql_UserName => Username;
    internal string Sql_Password => Password;

    public async Task InitializeAsync()
    {
        _mssqlContainer = new ContainerBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPortBinding(MsSqlPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithEnvironment("MSSQL_SA_PASSWORD", Password)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
            .WithName($"sql-test-{Guid.NewGuid():N}")
            .Build();

        await _mssqlContainer.StartAsync();

        var port = _mssqlContainer.GetMappedPublicPort(MsSqlPort);
        var connStr = $"Server=localhost,{port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=true;";

        DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(connStr)
            .Options;

        CurrentUserServiceMock = new Mock<ICurrentUserService>();
        CurrentUserServiceMock.Setup(s => s.UserId).Returns("test-user-id");

        using var dbContext = new ApplicationDbContext(DbContextOptions, CurrentUserServiceMock.Object);
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _mssqlContainer.DisposeAsync();
    }
}