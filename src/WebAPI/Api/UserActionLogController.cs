using Application.Features.User.Queries.GetUser;
using Application.Features.UserActionLog.Query.GetUserActionLog;
using Application.Features.UserActionLog.Query.GetUserActionLogById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Api
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class UserActionLogController : BaseApiController
    {
        private ILogger<UserController> _logger;
        public UserActionLogController(ILogger<UserController> logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// This endpoint retrieves a list of user action logs based on the provided query parameters.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("useractionloggers")]
        public async Task<IResult> GetUserActionLogAsync([FromQuery] GetUserActionLogQuery query)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for {@RequestData}", nameof(GetUserActionLogAsync), query);
            var response = await Mediator.Send(query);
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(GetUserActionLogAsync), response);
            return response;
        }


        /// <summary>
        /// This endpoint retrieves a specific user action log by its ID.
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        [HttpGet("useractionloggers/{logId:int}")]
        public async Task<IResult> GetUserActionLogByIdAsync(int logId)
        {
            _logger.LogInformation("{FunctionName} trigger function received a request for logId: {LogId}", nameof(GetUserActionLogByIdAsync), logId);
            var response = await Mediator.Send(new GetUserActionLogByIdQuery { LogId = logId });
            _logger.LogInformation("{FunctionName} trigger function returned a response {@ResponseData}", nameof(GetUserActionLogByIdAsync), response);
            return response;
        }
    }
}
