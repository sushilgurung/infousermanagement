

namespace Infrastructure.Persistence.ServiceRegister
{
    /// <summary>
    /// This class is responsible for registering database services in the ASP.NET Core dependency injection container.
    /// </summary>
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
