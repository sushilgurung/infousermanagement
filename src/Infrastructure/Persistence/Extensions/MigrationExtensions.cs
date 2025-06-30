using Infrastructure.Persistence.Data.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        app.Logger.LogInformation("{FunctionName} Seeding Database...", nameof(ApplyMigrationsAsync));
        using var serviceScope = app.Services.CreateAsyncScope();
        var scopedProvider = serviceScope.ServiceProvider;
        try
        {
            var dbcontext = scopedProvider.GetRequiredService<ApplicationDbContext>();
            if (dbcontext.Database.IsSqlServer())
            {
                await dbcontext.Database.EnsureCreatedAsync();
            }
            await DbContextSeed.InitializeDatabaseAsync(dbcontext, app);
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occurred seeding the DB.");
        }
    }
}


