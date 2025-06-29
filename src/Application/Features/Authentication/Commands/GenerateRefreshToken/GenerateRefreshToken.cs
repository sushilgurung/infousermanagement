using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Features.Authentication.Commands.GenerateRefreshToken;
using Application.Features.Authentication.Commands.Login;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Authentication.Command.GenerateRefreshToken;


public class GenerateRefreshToken : IRequest<IResult>
{
    [Required]
    public string Token { get; set; }

    [Required]
    public string RefreshToken { get; set; }

}

public class GenerateRefreshTokenCommandHandler : IRequestHandler<GenerateRefreshToken, IResult>
{
    private readonly ILogger<GenerateRefreshTokenCommandHandler> _logger;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private UserManager<ApplicationUser> _userManager;

    public GenerateRefreshTokenCommandHandler(
        ILogger<GenerateRefreshTokenCommandHandler> logger,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager
        )
    {
        _logger = logger;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<IResult> Handle(GenerateRefreshToken request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
         nameof(GenerateRefreshTokenCommandHandler), request);
        try
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            if (principal is null)
            {
                return Results.Json(
                                  Result.Failure("Invalid username or password."),
                                  statusCode: StatusCodes.Status401Unauthorized);
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var savedRefreshToken = await _refreshTokenRepository.FirstOrDefaultAsync(r => r.UserId == userId && r.Token == request.RefreshToken);
            if (savedRefreshToken is null)
            {
                return Results.Json(
                                  Result.Failure("Invalid refresh token."),
                                  statusCode: StatusCodes.Status401Unauthorized);
            }
            var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);
            var newAccessToken = _tokenService.GenerateToken(principal.Claims.ToList());
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = newRefreshToken,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });
            _logger.LogInformation("Refresh token generated for userId {UserId}.", userId);
            await _refreshTokenRepository.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            TokenResponseDto tokenResponse = new TokenResponseDto
            {
                Token= newAccessToken,
                RefreshToken = newRefreshToken,
            };

            return Results.Ok(Result.Success<TokenResponseDto>(data: tokenResponse, message: "User Token has been generate successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(GenerateRefreshTokenCommandHandler), request);
            return Results.Problem("Failed. Please try again later.", statusCode: 500);
        }
    }
}
