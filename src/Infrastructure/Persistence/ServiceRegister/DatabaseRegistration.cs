using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// Represents the registration of database-related services in an application.
    /// </summary>
    /// <remarks>This class is responsible for configuring and adding database services to the application's
    /// dependency injection container. It implements the <see cref="IDbServiceRegistration"/> interface, ensuring a
    /// consistent contract for service registration.</remarks>
    //public class DatabaseRegistration : IDbServiceRegistration
    //{
    //    public void AddServices(IServiceCollection services, IConfiguration configuration)
    //    {
    //        string connectionString = configuration.GetConnectionString("DefaultConnection");
    //        services.AddDbContext<ApplicationDbContext>(options =>
    //        {
    //            options.UseSqlServer(connectionString,
    //            sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
    //        });
    //    }
    //}

    public class DatabaseRegistration
    {
        public static void AddServices(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });
        }
    }
}
