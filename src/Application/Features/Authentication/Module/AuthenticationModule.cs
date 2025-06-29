using Application.Features.Authentication.Command.GenerateRefreshToken;
using Application.Features.Authentication.Command.Login;
using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Application.Features.Authentication.Module
{
    public class AuthenticationModule : CarterModule
    {
        public AuthenticationModule()
            : base("/api")
        {
            WithTags("Login");
            IncludeInOpenApi();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app = app.MapGroup("auth");
            app.MapPost("login", async (IMediator mediator, LoginCommand command) =>
            {
                return await mediator.Send(command);
            });

            app.MapPost("refresh-token", async (IMediator mediator, GenerateRefreshToken command) =>
            {
                return await mediator.Send(command);
            }).RequireAuthorization();
        }
    }
}
