using BlazorClient.Handlers;
using BlazorClient.Services.AuthService;
using BlazorClient.Services.UserActionLog;
using BlazorClient.Services.UserService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;


namespace BlazorClient
{
    public static class ServiceRegistration
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorizationCore();
            services.AddTransient<UnauthorizedRedirectHandler>();

            var url = configuration["WebApiAddress"];
            services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedRedirectHandler>();

            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedRedirectHandler>();


            services.AddHttpClient<IUserActionLogService, UserActionLogService>(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedRedirectHandler>();

            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, AuthService>();
            services.AddBlazoredLocalStorage();
            services.AddTransient<AuthHeaderHandler>();

            //builder.Services.AddTransient<CustomAuthMessageHandler>();
            //builder.Services.AddHttpClient("AuthAPI", client =>
            //{
            //    client.BaseAddress = new Uri("https://yourapi.com/");
            //}).AddHttpMessageHandler<CustomAuthMessageHandler>();
        }
    }
}
