using System.Net;
using System.Net.Http.Headers;
using BlazorClient.Dto;
using BlazorClient.Services.AuthService;
using Blazored.LocalStorage;

namespace BlazorClient.Handlers;

public class AuthMessageHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly IAuthService _authService;

    public AuthMessageHandler(ILocalStorageService localStorage, IAuthService authService)
    {
        _localStorage = localStorage;
        _authService = authService;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userDetails = await _localStorage.GetItemAsync<UserDetailsDto>("userDetails");
        var token =  userDetails.Token;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshSuccess = await _authService.TryRefreshTokenAsync();
            if (refreshSuccess.IsSuccess)
            {
                var newToken = refreshSuccess.Data.Token;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

                response.Dispose(); // Dispose old response
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }
}
