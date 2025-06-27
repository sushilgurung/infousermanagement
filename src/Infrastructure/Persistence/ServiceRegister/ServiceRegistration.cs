
using Application.Common.Behaviours;
using Application.Exceptions;
using Carter;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// 
    /// </summary>
    //public class ServiceRegistration : IServicesRegistrationWithConfig
    //{
    //    public void AddServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.AddScoped<ITokenService, TokenService>();
    //        services.AddScoped<ICurrentUserService, CurrentUserService>();
    //        services.AddScoped<IUserActionLogService, UserActionLogService>();
    //    }
    //}

    public class ServiceRegistration
    {
        public static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            RepositoriesRegistration.AddServices(services);
            DatabaseRegistration.AddServices(services, configuration);
            IdentityRegistration.AddServices(services, configuration);
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserActionLogService, UserActionLogService>();
        }


    }
}

