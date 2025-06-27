using BlazorClient.Common;
using BlazorClient.Dto;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text.Json;


namespace BlazorClient.Services.AuthService;

public class AuthService : AuthenticationStateProvider, IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthService> _logger;
    private readonly ILocalStorageService _localStorage;

    public AuthService(
        HttpClient httpClient,
        ILogger<AuthService> logger,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _logger = logger;
        _localStorage = localStorage;
    }

    /// <summary>
    /// This method is used to get the current authentication state of the user.
    /// </summary>
    /// <returns></returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userDetails = await _localStorage.GetItemAsync<UserDetailsDto>("userDetails");
        if (userDetails == null || string.IsNullOrWhiteSpace(userDetails.Token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        var claims = ParseClaimsFromJwt(userDetails.Token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
    /// <summary>
    /// This method is used to parse claims from a JWT token.
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(PadBase64(payload));
        var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return claims.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }
    private string PadBase64(string base64) =>
      base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    /// <summary>
    /// This method is used to log in a user by sending a POST request to the authentication endpoint.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public async Task<Result<UserDetailsDto>> LoginAsync(string username, string password)
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                 nameof(LoginAsync), new
                                 {
                                     UserName = username,
                                 });
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { username, password });
            if (response.IsSuccessStatusCode)
            {
                var userDetails = await response.Content.ReadFromJsonAsync<Result<UserDetailsDto>>();
                if (userDetails.IsSuccess)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userDetails?.Data.Token);
                    await _localStorage.SetItemAsync("userDetails", userDetails.Data);
                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                }
                return userDetails;
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                    var content = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<Result<UserDetailsDto>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return errorResponse;
                case HttpStatusCode.InternalServerError:
                    return Result.Failure<UserDetailsDto>("An unexpected server error occurred. Please contact the administrator.");
                default:
                    var reason = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Unexpected error ({StatusCode}): {Reason}", response.StatusCode, reason);
                    return Result.Failure<UserDetailsDto>($"An unexpected server error occurred. Please contact the administrator.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(LoginAsync), new
            {
                UserName = username,
            });
        }
        return new Result<UserDetailsDto>();
    }

    /// <summary>
    /// This method is used to notify the authentication state has changed, typically after a user logs in or out.
    /// </summary>
    /// <returns></returns>
    public async Task NotifyUserAuthenticationAsync()
    {
        var authenticationState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
    }

    /// <summary>
    /// This method is used to log out a user by removing the authentication token from local storage and clearing the HTTP client's authorization header.
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        _logger.LogInformation("Starting {FunctionName} with request: {@RequestData}",
                                 nameof(LogoutAsync), new { });
        try
        {
            await _localStorage.RemoveItemAsync("userDetails");
            _httpClient.DefaultRequestHeaders.Authorization = null;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()))));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{FunctionName} trigger function received a request for {@RequestData}", nameof(LogoutAsync), new { });
        }
    }
}

