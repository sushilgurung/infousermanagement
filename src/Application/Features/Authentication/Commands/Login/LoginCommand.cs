using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Features.Authentication.Commands.Login;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Command.Login;


public record LoginCommand(string UserName, string Password) : IRequest<IResult>;


public class LoginCommandHandler : IRequestHandler<LoginCommand, IResult>
{
    private readonly ILogger<LoginCommandHandler> _logger;
    private readonly ITokenService _tokenService;

    private UserManager<ApplicationUser> _userManager;
    private RoleManager<ApplicationRole> _roleManager;
    private SignInManager<ApplicationUser> _signInManager;


    private readonly IUserActionLogService _userActionLogService;


    private readonly IRefreshTokenRepository _refreshTokenRepository;
    public LoginCommandHandler(
        ILogger<LoginCommandHandler> logger,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IUserActionLogService userActionLogService,
         IRefreshTokenRepository refreshTokenRepository
        )
    {
        this._logger = logger;
        this._tokenService = tokenService;
        this._userManager = userManager;
        this._roleManager = roleManager;
        this._signInManager = signInManager;
        this._userActionLogService = userActionLogService;
        this._refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<IResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
         nameof(LoginCommandHandler), request);
        try
        {
            var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false);
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(request.UserName).ConfigureAwait(false);
            }
            if (user is null)
            {
                return Results.Json(
                        Result.Failure("Invalid username or password."),
                        statusCode: StatusCodes.Status401Unauthorized);
            }
            SignInResult result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false).ConfigureAwait(false);
            if (result.Succeeded is false)
            {
                return Results.Json(
                    Result.Failure("Invalid username or password."),
                    statusCode: StatusCodes.Status401Unauthorized);
            }
            IList<string> roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            IList<Claim> userClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);

            var roleClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim("roles", role));
            }

            var claims = new List<Claim> {
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("UserName", user.UserName)
                            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = _tokenService.GenerateToken(claims.ToList());
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow
            });
            _logger.LogInformation("Refresh token generated for user {UserName}.", user.UserName);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            LoginCommandDto loginCommandDto = new LoginCommandDto
            {
                UserName = user.UserName,
                Token = token,
                Roles = roles.ToArray(),
                RefreshToken = refreshToken
            };

            _logger.LogInformation("User {UserName} has been logged in successfully.", user.UserName);
            return Results.Ok(Result.Success<LoginCommandDto>(data: loginCommandDto, message: "User has been login successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(LoginCommandHandler), request);
            return Results.Problem(new
            {
                success = false,
                Message = "An unexpected error occurred while processing your request. Please try again later.",
                Error = ex.Message
            }.ToString(),
            title: "Internal Server Error",
            statusCode: 500);
        }
    }


    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(v => v.UserName).NotEmpty().WithMessage("ForName is Required");
            RuleFor(v => v.Password).NotEmpty().WithMessage("ForName is Required");
        }
    }

}
