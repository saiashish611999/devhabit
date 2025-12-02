using devhabit.api.Database;
using Microsoft.EntityFrameworkCore;

namespace devhabit.api.Extensions;

public static class DatabaseMigrations
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();

            app.Logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "An error occured while applying migrations");
            throw;
        }
    }
}
