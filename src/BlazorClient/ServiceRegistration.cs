using BlazorClient.Handlers;
using BlazorClient.Services.AuthService;
using BlazorClient.Services.UserActionLog;
using BlazorClient.Services.UserService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

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
            //.AddHttpMessageHandler<AuthMessageHandler>();

            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedRedirectHandler>();
            //.AddHttpMessageHandler<AuthMessageHandler>();


            services.AddHttpClient<IUserActionLogService, UserActionLogService>(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .AddHttpMessageHandler<UnauthorizedRedirectHandler>();
            //.AddHttpMessageHandler<AuthMessageHandler>();

            services.AddTransient<AuthHeaderHandler>();
            services.AddTransient<AuthMessageHandler>();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, AuthService>();
            services.AddBlazoredLocalStorage();

             
        }
    }
}
