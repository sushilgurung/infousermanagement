
using Application.Interfaces.QueueServices;
using Infrastructure.Persistence.QueueServices;

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// This class is responsible for registering persistence-related services in the ASP.NET Core dependency injection container.
    /// </summary>
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityRegistration.AddServices(services, configuration);
            DatabaseRegistration.AddServices(services, configuration);
            RepositoriesRegistration.AddServices(services);
            services.AddServices(configuration);
        }

        /// <summary>
        /// This method registers services.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserActionLogService, UserActionLogService>();
            services.AddScoped<IIdentityOptionsAccessorService, IdentityOptionsAccessorService>();

            services.AddScoped<IQueueService, QueueService>();
            services.AddTransient<IUserActionLogQueuePublisher, UserActionLogQueuePublisher>();
            services.AddSingleton<IUserActionLogQueueConsumer, UserActionLogQueueConsumer>();
        }


    }
}

