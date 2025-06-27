using Application.Features.User.Command.DeleteUser;
using Application.Features.User.Command.UpdateUser;
using Application.Features.User.Commands.CreateUser;
using Application.Features.User.Queries.GetUser;
using Application.Features.User.Query.GetUserById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace WebAPI.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseApiController
    {
        private ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// This endpoint retrieves a list of users based on the provided command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public async Task<IResult> GetUserManagementAsync([FromQuery] GetUserQuery query)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserManagementAsync), query);
            var response = await Mediator.Send(query);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(GetUserManagementAsync), response);
            return response;
        }


        /// <summary>
        /// This endpoint creates a new user management record based on the provided command.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResult> CreateUserManagementAsync([FromBody] CreateUserCommand command)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {@RequestData}", nameof(CreateUserManagementAsync), command);
            var response = await Mediator.Send(command);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(CreateUserManagementAsync), response);
            return response;
        }

        /// <summary>
        /// This endpoint updates a user management record based on the provided user ID and command.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{userId:int}")]
        public async Task<IResult> UpdateUserManagementAsync(int userId, [FromBody] UpdateUserCommand command)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {@RequestData}", nameof(UpdateUserManagementAsync), command);
            command.UserId = userId;
            var response = await Mediator.Send(command);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(UpdateUserManagementAsync), response);
            return response;
        }

        /// <summary>
        /// This endpoint retrieves a user by their ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId:int}")]
        public async Task<IResult> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for user ID: {UserId}", nameof(GetUserByIdAsync), userId);
            var query = new GetUserByIdQuery { UserId = userId };
            var response = await Mediator.Send(query);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(GetUserByIdAsync), response);
            return response;
        }

        [HttpDelete("{userId:int}")]
        public async Task<IResult> DeleteUserAsync(int userId)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request to delete user ID: {UserId}", nameof(DeleteUserAsync), userId);
            var command = new DeleteUserCommand { UserId = userId };
            var response = await Mediator.Send(command);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(DeleteUserAsync), response);
            return response;
        }

    }
}