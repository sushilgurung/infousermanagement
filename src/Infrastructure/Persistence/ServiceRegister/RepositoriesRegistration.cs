

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// This class is responsible for registering repository services in the ASP.NET Core dependency injection container.
    /// </summary>
    public class RepositoriesRegistration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserActionLogRepository, UserActionLogRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
    }


}
