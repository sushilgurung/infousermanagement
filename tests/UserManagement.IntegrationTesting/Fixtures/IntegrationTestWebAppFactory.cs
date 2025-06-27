using Application.Interfaces.Services;
using Docker.DotNet.Models;
using Domain.Entities;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Infrastructure.Persistence.Contexts; // Your DbContext namespace
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Testcontainers.MsSql;
using UserManagement.IntegrationTesting.Authentication;
using UserManagement.IntegrationTesting.Data;
namespace UserManagement.IntegrationTesting.Fixtures
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public IConfiguration Configuration { get; private set; }
        private const int MsSqlPort = 1433;
        private const string Username = "sa";
        private const string Password = "P@ssw0rd!2025#Secure";
        private const string DatabaseName = "Test-UserManagement";

        internal string HostName => _mssqlContainer.Hostname;
        internal int Port => _mssqlContainer.GetMappedPublicPort(MsSqlPort);
        internal string Sql_UserName => Username;
        internal string Sql_Password => Password;

        public IContainer _mssqlContainer { get; private set; }
        public DbContextOptions<ApplicationDbContext> DbContextOptions { get; private set; }
        public Mock<ICurrentUserService> CurrentUserServiceMock { get; private set; }
        public IntegrationTestWebAppFactory()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _mssqlContainer = new ContainerBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPortBinding(MsSqlPort, true)
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("MSSQL_SA_PASSWORD", Password)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MsSqlPort))
        .WithName($"sql-test-{Guid.NewGuid():N}")
        .Build();
        }
        public async Task InitializeAsync()
        {
            try
            {
                await _mssqlContainer.StartAsync();

                //var port = _mssqlContainer.GetMappedPublicPort(MsSqlPort);
                //var connStr = $"Server=localhost,{port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=true;";

                //DbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                //    .UseSqlServer(connStr)
                //    .Options;

                //using var dbContext = new ApplicationDbContext(DbContextOptions, CurrentUserServiceMock.Object);
                //await dbContext.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting container: {ex.Message}");
                var logs = await _mssqlContainer.GetLogsAsync();
                Console.WriteLine($"Container logs:\n{logs}");
                throw;
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var port = _mssqlContainer.GetMappedPublicPort(MsSqlPort);
            var ConnectionString = $"Server=localhost,{port};Database={DatabaseName};User Id={Username};Password={Password};TrustServerCertificate=true;";
            builder.ConfigureTestServices(async services =>
            {

                var descriptor = services
                   .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }
                //services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
                //services.RemoveAll(typeof(ApplicationDbContext));
              
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(ConnectionString));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbcontext.Database.Migrate();

                if (dbcontext.Database.IsSqlServer())
                {
                    await dbcontext.Database.EnsureCreatedAsync();
                }


                //services.RemoveAll(typeof(ICurrentUserService));
                //services.AddSingleton(CurrentUserServiceMock.Object);


                await TestDataSeeder.SeedTestUserAsync(sp);
                //services.AddAuthentication("Test")
                //   .AddScheme<AuthenticationSchemeOptions, FakeJwtAuthHandler>("Test", options => { });

                //services.AddAuthorization(options =>
                //{
                //    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
                //});
                builder.Configure(app =>
                {
                    app.UseAuthentication();
                    app.UseAuthorization();
                });
            });
        }


        public async Task<HttpClient> CreateClientWithJwtAsync()
        {

            using var scope = Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var configuration = Configuration;
            var user = await userManager.FindByEmailAsync("testuser@example.com");
            if (user == null)
                throw new Exception("Seeded test user not found.");

            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));

            // Add standard JWT claims if needed

            var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("UserName", user.UserName)
                            }
                     .Union(userClaims)
                     .Union(roleClaims);
            var token = JwtTokenHelper.GenerateJwtToken(configuration, claims);

            var client = this.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task DisposeAsync()
        {
            if (_mssqlContainer != null)
            {
                await _mssqlContainer.StopAsync();
                await _mssqlContainer.DisposeAsync();
            }
        }
    }
}
