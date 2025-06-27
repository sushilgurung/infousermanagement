using BlazorClient.Dto;
using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BlazorClient.Handlers
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var userDetails = await _localStorage.GetItemAsync<UserDetailsDto>("userDetails");

            if (userDetails != null && !string.IsNullOrWhiteSpace(userDetails.Token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", userDetails.Token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

}
