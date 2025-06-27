using Application.Features.Authentication.Commands.Login;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UserManagement.IntegrationTesting.Fixtures;
using Domain.Common;
namespace UserManagement.IntegrationTesting.Controllers
{
    [Collection("IntegrationTestCollection")]
    public class AuthControllerTests : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(IntegrationTestWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new
            {
                UserName = "testuser@example.com",
                Password = "YourTestPassword123!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            // var json = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            var loginResponse = await response.Content.ReadFromJsonAsync<Result<LoginCommandDto>>();

            loginResponse.Should().NotBeNull();
            loginResponse.IsSuccess.Should().BeTrue();
            loginResponse.Data.Should().NotBeNull();
            loginResponse.Data.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new
            {
                UserName = "fakeuser@example.com",
                Password = "WrongPassword!"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
            var loginResponse = await response.Content.ReadFromJsonAsync<Result<LoginCommandDto>>();
            loginResponse.IsSuccess.Should().BeFalse();
            loginResponse.Data.Should().BeNull();
            loginResponse.Data.Token.Should().BeNullOrEmpty();
        }
    }
}
