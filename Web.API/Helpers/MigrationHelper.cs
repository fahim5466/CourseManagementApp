using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Web.API.Helpers
{
    public static class MigrationHelper
    {
        /// <summary>
        /// Applies any pending migrations to the database.
        /// </summary>
        /// <param name="app"></param>
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using ApplicationDbContext dbContext =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Check and apply pending migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
}
