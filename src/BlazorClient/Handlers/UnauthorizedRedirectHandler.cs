using Microsoft.AspNetCore.Components;

namespace BlazorClient.Handlers
{
    public class UnauthorizedRedirectHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;

        public UnauthorizedRedirectHandler(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("/login");
            }
            return response;
        }
    }
}
