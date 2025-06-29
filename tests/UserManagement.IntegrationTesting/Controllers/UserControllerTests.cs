using Application.Features.User.Command.UpdateUser;
using Application.Features.User.Commands.CreateUser;
using Application.Features.User.Queries.GetUser;
using Bogus.Bson;
using Domain.Common;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using UserManagement.IntegrationTesting.Fixtures;
using UserManagement.IntegrationTesting.Helper;

namespace UserManagement.IntegrationTesting.Controllers
{
    [Collection("IntegrationTests")]
    public class UserControllerTests : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private HttpClient _client;

        public UserControllerTests(IntegrationTestWebAppFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            // _client =  _factory.CreateClientWithJwtAsync().Result;
        }


        [Fact]
        public async Task GetUserManagementAsync_ShouldReturnUsers()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();
            var query = new GetUserRequestParameters
            {
                PageNumber = 1,
                PageSize = 10,
            };

            // Act
            var response = await _client.GetAsync($"/api/user/users{query.GetQueryString()}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Result<PaginatedList<GetUserDto>>>(json);
            //var users = await response.Content.ReadFromJsonAsync<Result<PaginatedList<GetUserDto>>>(); 
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateUserManagementAsync_ShouldCreateUser()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();

            var createCommand = new CreateUserDto
            {
                ForeName = "testuserCreate",
                SurName = "SurNameTest",
                Email = "testuserCreate@example.com",
                DateOfBirth = new DateOnly(1990, 1, 1),
                IsActive = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/user", createCommand);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Result>();
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUserManagementAsync_ShouldUpdateUser()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();
            int userId = 1;
            var updateCommand = new UpdateUserCommand
            {
                ForeName = "Edit_ForeName",
                SurName = "Edit_SurName",
                Email = "Edited@yopmail.com",
                DateOfBirth = new DateOnly(1990, 1, 1),
                IsActive = false
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/user/{userId}", updateCommand);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Result>();
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();
            int userId = 1;

            // Act
            var response = await _client.GetAsync($"/api/user/{userId}");
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<Result>(); // Adjust to actual user DTO
            user.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();
            int userId = 5;

            // Act
            var response = await _client.DeleteAsync($"/api/user/{userId}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Result>(); // Adjust to actual response DTO
            result.Should().NotBeNull();
        }



        [Fact]
        public async Task Should_Have_Error_When_ForeName_Is_Empty()
        {
            // Arrange
            _client = await _factory.CreateClientWithJwtAsync();

            var createCommand = new CreateUserDto
            {
                ForeName = "",
                SurName = "",
                Email = "",
                DateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddYears(-30)),
                IsActive = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/user", createCommand);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            var json = await response.Content.ReadAsStringAsync();
            var result = await response.Content.ReadFromJsonAsync<Result>();

            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeNullOrEmpty();

            result.Errors.Should().ContainKey("ForeName");
            result.Errors["ForeName"].Should().Contain("ForName is Required");

            result.Errors.Should().ContainKey("SurName");
            result.Errors["SurName"].Should().Contain("ForName is Required");

            result.Errors.Should().ContainKey("Email");
            result.Errors["Email"].Should().Contain(new[] { "ForName is Required", "Invalid Email Address" });
        }

    }
}
