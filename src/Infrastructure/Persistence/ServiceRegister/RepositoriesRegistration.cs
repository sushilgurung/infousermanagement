

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// 
    /// </summary>
    //public class RepositoriesRegistration : IRepositoriesRegistration
    //{
    //    public void AddServices(IServiceCollection services)
    //    {

    //    services.AddScoped<IUserRepository, UserRepository>();
    //    services.AddScoped<IUserActionLogRepository, UserActionLogRepository>();
    //    }
    //}

    public  class RepositoriesRegistration
    {
        public static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserActionLogRepository, UserActionLogRepository>();
        }
    }


}
