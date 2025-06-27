using Application.Common.Behaviours;
using Application.Exceptions;
using Carter;
using FluentValidation;
using Gurung.ServiceRegister;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    //public class ApplicationServiceRegistration : IServicesRegistrationWithConfig
    //{
    //    public void AddServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
    //        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
    //    }
    //}
    public static class ApplicationServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), ServiceLifetime.Scoped);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddSingleton<ApiExceptionFilter>();
            services.AddControllersWithViews(options => options.Filters.Add(new ApiExceptionFilter()));
            services.AddCarter(configurator: c =>
            {
                c.WithValidatorLifetime(ServiceLifetime.Scoped);
            });
        
        }
    }
}
