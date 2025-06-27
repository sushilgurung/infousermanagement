
using Domain.Enum;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Data.Seeding.FakeDataGenerator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace Infrastructure.Persistence.Data.Seeding;

public class DbContextSeed
{
    public static async Task InitializeDatabaseAsync(ApplicationDbContext applicationDbContext, WebApplication app)
    {
        using var serviceScope = app.Services.CreateAsyncScope();
        var scopedProvider = serviceScope.ServiceProvider;
        var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scopedProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManagementRepository = scopedProvider.GetRequiredService<IUserRepository>();
        //if (app.Environment.IsDevelopment())
        //{
            await AddRoles(roleManager, app.Logger);
            await AddAdmin(userManager, app.Logger);
       // }
        await SeedUserManagementAsync(userManagementRepository, applicationDbContext, app.Logger).ConfigureAwait(false);
    }

    /// <summary>
    /// This method adds roles to the RoleManager if they do not already exist.
    /// </summary>
    /// <param name="roleManager"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static async Task AddRoles(RoleManager<ApplicationRole> roleManager, ILogger logger)
    {

        List<ApplicationRole> roles = new List<ApplicationRole>(){
                     new ApplicationRole(nameof(RolesEnum.SuperUser))
                };
        foreach (var role in roles)
        {
            var existingRole = await roleManager.FindByNameAsync(role.Name).ConfigureAwait(false);
            if (existingRole is null)
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    /// <summary>
    /// This method adds a SuperUser to the UserManager if it does not already exist.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static async Task AddAdmin(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        var user = await userManager.FindByNameAsync(nameof(RolesEnum.SuperUser)).ConfigureAwait(false);
        if (user is null)
        {
            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = nameof(RolesEnum.SuperUser),
                Email = "SuperUser@yopmail.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(applicationUser, "SuperUser@123").ConfigureAwait(false);
            if (result.Succeeded)
            {
                logger.LogInformation("SuperUser created successfully.");
                await userManager.AddToRoleAsync(applicationUser, nameof(RolesEnum.SuperUser));
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    logger.LogError("Error creating SuperUser: {ErrorDescription}", error.Description);
                }
            }
        }
    }

    /// <summary>
    /// This method seeds the UserManagement table with fake data using Bogus library.
    /// </summary>
    /// <param name="userManagementRepository"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    public static async Task SeedUserManagementAsync(IUserRepository userManagementRepository, ApplicationDbContext applicationDbContext, ILogger logger)
    {
        logger.LogInformation("{FunctionName} started seeding users.", nameof(SeedUserManagementAsync));
        try
        {
            var entityType = applicationDbContext.Model.FindEntityType(typeof(User));
            if (entityType == null)
                throw new InvalidOperationException("Entity type 'UserManagement' not found in DbContext model.");
            string tableName = entityType.GetTableName();

            var faker = DataGenerator.GenerateUserManagements();
            var users = faker.Generate(100);

            var usersCount = applicationDbContext.UsersManagement.CountAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (usersCount <= 0)
            {
                await applicationDbContext.AddRangeAsync(users).ConfigureAwait(false);

                await applicationDbContext.Database.OpenConnectionAsync();
                await applicationDbContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [dbo].[{tableName}] ON;").ConfigureAwait(false);

                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);

                await applicationDbContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT  [dbo].[{tableName}] OFF;");
                await applicationDbContext.Database.CloseConnectionAsync();
            }
            logger.LogInformation("Seeding completed. new users were added.");
        }
        catch (Exception)
        {
            throw;
        }
    }
}
